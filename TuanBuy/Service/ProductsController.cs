using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data.Common;
using System.Linq;
using System.Net.NetworkInformation;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TuanBuy.Models;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

namespace TuanBuy.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IRepository<Product> _productsRepository;
        private readonly IRepository<User> _userRepository;
        private readonly TuanBuyContext _dbContext;
        public ProductsController(GenericRepository<Product> productsRepository, IWebHostEnvironment environment,
            GenericRepository<User> userRepository, TuanBuyContext dbContext)
        {
            _productsRepository = productsRepository;
            _environment = environment;
            _userRepository = userRepository;
            _dbContext = dbContext;
        }

        //我的商品
        [Route("MyProducts")]
        [HttpGet]
        public ActionResult<IEnumerable<ProductViewModel>> GetMyProducts()
        {
            var targetUser = GetTargetUser();

            var product = GetAllProducts();
            //只取第一張圖片
            var products = new List<ProductViewModel>();
            foreach (var p in product)
            {
                if (p.User == targetUser && p.Disable == false)
                {
                    var prod = new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Content = p.Content,
                        Category = p.Category,
                        PicPath = p.PicPath,
                        EndTime = p.EndTime,
                        Price = p.Price,
                        Href = p.Href
                    };
                    products.Add(prod);
                }
            }

            return products;

        }



        //首頁撈商品
        [HttpGet]
        public ActionResult<IEnumerable<ProductViewModel>> GetProducts()
        {
            #region 原始版本 可以的
            //var products = GetAllProducts();
            //var result = products.Where(p => p.Disable == false)
            //    .Select(p => new ProductViewModel
            //    {
            //        Id = p.Id,
            //        Name = p.Name,
            //        Description = p.Description,
            //        Content = p.Content,
            //        Category = p.Category,
            //        PicPath = p.PicPath,
            //        EndTime = p.EndTime,
            //        Price = p.Price,
            //        Href = p.Href
            //    })
            //    .OrderByDescending(x => x.Id)
            //    .ToList();
            //return result;
            #endregion

            #region 測試
            //var result =
            //    from p in _dbContext.Product
            //    join pic in _dbContext.ProductPics on p.Id equals pic.Id //into prodPic
            //    join o in _dbContext.Order on p.Id equals o.Id
            //    join od in _dbContext.OrderDetail on o.Id equals od.Id
            //    where p.Disable == false
            //    orderby p.Id
            //    select new ProductViewModel
            //    {
            //        Id = p.Id,
            //        Total = p.Price * od.Count,
            //        Price = p.Price,
            //        Description = p.Description,
            //        Content = p.Content,
            //        Category = p.Category,
            //        PicPath = "/productpicture/" + pic.PicPath,
            //        EndTime = p.EndTime,
            //        Href = "/Product/DemoProduct/" + p.Id
            //    };
            //var test = result.ToList();

            //var result = (from product in _dbContext.Product
            //    join prdpics in _dbContext.ProductPics on product.Id equals prdpics.ProductId
            //    select new {product, prdpics}).ToList();


            #endregion
            var products = GetAllProducts().OrderByDescending(x=>x.Id);
            var orderDetails =
                (from orderDetail in _dbContext.OrderDetail
                    //where (products.Select(x => x.Id)).Contains(orderDetail.ProductId)
                 select new {orderdetail = orderDetail }).ToList();
            var result = new List<ProductViewModel>();
            foreach (var p in products)
            {
                var i = new ProductViewModel();
                i.Id = p.Id;
                i.Name = p.Name;
                i.Description = p.Description;
                i.Content = p.Content;
                i.Category = p.Category;
                i.PicPath = p.PicPath;
                TimeSpan timeSpan = p.EndTime.Subtract(DateTime.Now).Duration();
                i.LastTime = timeSpan.Days + "天";
                i.Price = p.Price;
                i.Total = 0;
                i.Href = p.Href;
                i.TargetPrice = p.TargetPrice;
                i.Color = "#3366a9";
                foreach (var orderDetail in orderDetails)
                {
                    if (orderDetail.orderdetail.ProductId == p.Id)
                    {
                        i.Total += orderDetail.orderdetail.Count * p.Price;
                    }
                }

                if (i.Total != null && i.Total != 0 &&i.TargetPrice!= 0)
                {
                    var a = (i.Total / i.TargetPrice) * 100;
                    if (a>=100)
                    {
                        a = 100;
                    }
                    i.Color = GetBarColor.GetColor(a);
                    i.Percentage = a + "%";
                }
                else
                {
                    i.Percentage = "0%";
                
                }
                if (i.TargetPrice == 0)
                {
                    i.Percentage = "100%";
                }

                result.Add(i);
            }

            return result;

        }


        [Route("HotProducts")]
        [HttpGet]
        public ActionResult<IEnumerable<ProductViewModel>> GetHotProducts()
        {
            var products = GetAllProducts().OrderByDescending(x => x.Id);
            var orderDetails =
                (from orderDetail in _dbContext.OrderDetail
                     //where (products.Select(x => x.Id)).Contains(orderDetail.ProductId)
                 select new { orderdetail = orderDetail }).ToList();
            var result = new List<ProductViewModel>();
            foreach (var p in products)
            {
                var i = new ProductViewModel();
                i.Id = p.Id;
                i.Name = p.Name;
                i.Description = p.Description;
                i.Content = p.Content;
                i.Category = p.Category;
                i.PicPath = p.PicPath;
                TimeSpan timeSpan = p.EndTime.Subtract(DateTime.Now).Duration();
                i.LastTime = timeSpan.Days + "天";
                i.Price = p.Price;
                i.Total = 0;
                i.Href = p.Href;
                i.TargetPrice = p.TargetPrice;
                i.Color = "#3366a9";
                foreach (var orderDetail in orderDetails)
                {
                    if (orderDetail.orderdetail.ProductId == p.Id)
                    {
                        i.Total += orderDetail.orderdetail.Count * p.Price;
                    }
                }

                if (i.Total != null && i.Total != 0 && i.TargetPrice != 0)
                {
                    var a = (i.Total / i.TargetPrice) * 100;
                    if (a >= 100)
                    {
                        a = 100;
                    }
                    i.Color = GetBarColor.GetColor(a);
                    i.Percentage = a + "%";

                    if (a >= 60)
                    {
                        i.Category = "精選";
                        result.Add(i);
                    }
                }
                else
                {
                    i.Percentage = "0%";

                }
                if (i.TargetPrice == 0)
                {
                    i.Percentage = "100%";
                }
            }

            return result;

        }


        //修改商品
        [HttpPut]
        public IActionResult PutProduct([FromBody] UpDateProductViewModel product)
        {
            var p = _productsRepository.Get(a => a.Id == Convert.ToInt32(product.Id));
            p.Price = product.Price;
            p.Content = product.Content;
            p.Description = product.Description;
            p.EndTime = product.EndTime;
            _productsRepository.SaveChanges();

            return Ok();
        }



        //商品軟刪除
        [HttpDelete("{id}")]
        public IActionResult DeleteProduct(int id)
        {
            var product = _productsRepository.Get(p => p.Id == id);
            if (product == null) return NotFound();
            product.Disable = true;
            _productsRepository.SaveChanges();
            return NoContent();
        }

        private bool ProductExists(Product product)
        {
            var b = _productsRepository.Get(p => p == product);
            if (b == null) return true;

            return false;
        }

        //抓取當前User
        private User GetTargetUser()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _userRepository.Get(a => a.Email == userEmail);
            return targetUser;
        }
        //抓取全部商品
        private List<ProductViewModel> GetAllProducts()
        {
            var product = _dbContext.Product.Where(x=>x.Disable ==false).ToList().GroupJoin(
                _dbContext.ProductPics.ToList(),
                product => product,
                productPic => productPic.Product,
                (p, pic) => new ProductViewModel
                {
                    User = p.User,
                    Disable = p.Disable,
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Content = p.Content,
                    Category = p.Category,
                    PicPath = "/productpicture/" + pic.FirstOrDefault()?.PicPath,
                    EndTime = p.EndTime,
                    Price = p.Price,
                    //目標金額是商品的Total欄位
                    TargetPrice = p.Total,
                    Href = "/Product/DemoProduct/" + p.Id
                }
            ).ToList();
            return product;
        }

    }
}