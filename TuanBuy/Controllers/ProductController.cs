using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Org.BouncyCastle.Crypto.Tls;
using StackExchange.Redis;
using TuanBuy.Models;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;
using Order = TuanBuy.Models.Entities.Order;

namespace TuanBuy.Controllers
{
    public class ProductController : Controller
    {
        private readonly IRepository<Product> _productsRepository;
        private readonly IWebHostEnvironment _environment;
        private readonly IRepository<User> _userRepository;
        private readonly TuanBuyContext _dbContext;
        private static IDistributedCache _distributedCache;
        private readonly RedisProvider _redisDb;

        public ProductController(GenericRepository<Product> productsRepository, IWebHostEnvironment environment, GenericRepository<User> userRepository, TuanBuyContext dbContext, IDistributedCache distributedCache, RedisProvider redisDb)
        {
            _productsRepository = productsRepository;
            _environment = environment;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _distributedCache = distributedCache;
            _redisDb = redisDb;

        }
        //新增商品首頁
        [Authorize(Roles = "FullUser")]
        public IActionResult Index()
        {
            return View();
        }
        //新增商品頁
        public IActionResult Test()
        {
            return View();
        }

        ////商品介紹頁
        //public IActionResult DemoProduct()
        //{
        //    return View();
        //}
        #region 商品介紹頁
        [HttpGet]
        public IActionResult DemoProduct(int id)
        {
            ProductManage product = new ProductManage(_dbContext);
            var result = product.GetDemoProductData(id);
            return View(result);
        }
        #endregion

        #region 取得商品頁資料
        [HttpGet]
        public DemoProductViewModel GetProductData(int id)
        {
            ProductManage product = new ProductManage(_dbContext);
            var result = product.GetDemoProductData(id);
            return result;
        }
        #endregion

        #region 取得商品頁留言
        [HttpGet]
        public List<ProductMessageViewModel> GetProductMessage(int id)
        {
            ProductManage product = new ProductManage(_dbContext);
            var result = product.GetProductMessageData(id);
            return result;
        }
        [HttpPost]
        #endregion 

        #region 新增產品頁留言
        public IActionResult AddProductMessage(int ProductId, int UserId, string MessageContent)
        {
            ProductManage product = new ProductManage(_dbContext);
            //新增商品頁留言
            product.AddProductMessage(ProductId, UserId, MessageContent);
            return Ok();
        }
        #endregion

        #region 賣家回覆留言
        public IActionResult AddSellerMessage(int ProductMessageId, int SellerId, string MessageContent)
        {
            ProductManage product = new ProductManage(_dbContext);
            //新增商品頁留言
            product.AddSellerMessage(ProductMessageId, SellerId, MessageContent);
            return Ok();
        }

        #endregion

        #region 將商品加入購物車
        [Authorize(Roles = "FullUser")]
        public void AddProductOrder(int ProductId, int UserId,int ProductCount)
        {
            var productData = (from product in _dbContext.Product
                               join productpic in _dbContext.ProductPics on product.Id equals productpic.ProductId
                               where product.Id == ProductId
                               select new { product, productpic }).FirstOrDefault();


            var userData = _dbContext.User.FirstOrDefault(x => x.Id == UserId);



            #region 原本session
            if (HttpContext.Session.GetString("ShoppingCart") != null) 
            {
                var shoppjson = HttpContext.Session.GetString("ShoppingCart");
                var shoppingcarts = JsonConvert.DeserializeObject<List<ProductCheckViewModel>>(shoppjson);

                //如果購物車商品重複則只重新加數量不加商品
                if(shoppingcarts.Any(x=>x.ProductId== ProductId))
                {
                    shoppingcarts.FirstOrDefault(x => x.ProductId == ProductId).ProductCount += ProductCount;
                }
                else
                {
                    //將使用者資訊存入session
                    //將先前購物車紀錄加入
                    shoppingcarts.Add(new ProductCheckViewModel
                    {
                        ProductId = productData.product.Id,
                        ProductPicPath = productData.productpic.PicPath,
                        ProductPrice = productData.product.Price,
                        ProductDescription = productData.product.Description,
                        ProductCount = ProductCount,
                        BuyerId = UserId,
                        BuyerName = userData.Name,
                        BuyerPhone = userData.Phone,
                        BuyerAddress = userData.Address
                    });
                }


                //先將先前session清除
                HttpContext.Session.Remove("ShoppingCart");
                //重新寫入新session
                HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(shoppingcarts));
            }
            else
            {
                var jsonstring = JsonConvert.SerializeObject(new List<ProductCheckViewModel>
                {
                   new ProductCheckViewModel
                   {
                    ProductId = productData.product.Id,
                    ProductPicPath = productData.productpic.PicPath,
                    ProductPrice = productData.product.Price,
                    ProductDescription = productData.product.Description,
                    ProductCount = ProductCount,
                    BuyerId = UserId,                  
                    BuyerName = userData.Name,
                    BuyerPhone = userData.Phone,
                    BuyerAddress = userData.Address
                   },
                });
                HttpContext.Session.SetString("ShoppingCart", jsonstring);


            }

            #endregion

        }
        #endregion

        #region 確認收藏、加入收藏、瀏覽收藏Redis

        public bool CheckFavicon(int id)
        {
            var claim = HttpContext.User.Claims;
            var userId = claim.FirstOrDefault(a => "Userid" == a.Type)?.Value;

            if (userId == null) return false;
            var db = _redisDb.GetRedisDb(2);
            var userShopCar = RedisProvider.ConvertToDictionaryInt((db.HashGetAll(userId)));
            var check = userShopCar.ContainsKey(id);

            return check;
        }
        public bool AddFavicon(int id)
        {
            var claim = HttpContext.User.Claims;
            var userId = claim.FirstOrDefault(a => "Userid" == a.Type)?.Value;
            if (userId == null) return false;

            var db = _redisDb.GetRedisDb(2);
            bool check;

            var userShopCar = RedisProvider.ConvertToDictionaryInt((db.HashGetAll(userId)));
            if (userShopCar.ContainsKey(id))
            {
                db.HashDelete(userId, id);
                check = false;
                //userShopCar[productId]++;
            }
            else
            {
                userShopCar.Add(id, 1);
                var shopCar = RedisProvider.ToHashEntryArray(userShopCar);
                db.HashSet(userId, shopCar);
                check = true;
            }

            return check;

        }

        public List<ProductViewModel> GetAllFavicon()
        {
            var claim = HttpContext.User.Claims;
            var userId = claim.FirstOrDefault(a => "Userid" == a.Type)?.Value;
            var db = _redisDb.GetRedisDb(2);
           
            var userShopCar = RedisProvider.ConvertToDictionaryInt((db.HashGetAll(userId)));
            var keyList = userShopCar.Select(i => i.Key).ToList();
            var productList = _dbContext.Product.Where(x => x.Disable == false & keyList.Contains(x.Id)).ToList().GroupJoin(
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


            var orderDetails =
                (from orderDetail in _dbContext.OrderDetail
                    //where (products.Select(x => x.Id)).Contains(orderDetail.ProductId)
                    select new { orderdetail = orderDetail }).ToList();
            var result = new List<ProductViewModel>();
            foreach (var p in productList)
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


        #endregion

        #region 刪除購物車商品
        public void DelectShoppingCart(int productId)
        {
            //刪除使用者購物車商品
            if (HttpContext.Session.GetString("ShoppingCart") != null)
            {
                var shoppjson = HttpContext.Session.GetString("ShoppingCart");
                var shoppingcarts = JsonConvert.DeserializeObject<List<ProductCheckViewModel>>(shoppjson);
                shoppingcarts.Remove(shoppingcarts.FirstOrDefault(x => x.ProductId == productId));
                //先將先前session清除
                HttpContext.Session.Remove("ShoppingCart");
                //重新寫入新session
                HttpContext.Session.SetString("ShoppingCart", JsonConvert.SerializeObject(shoppingcarts));



            }
        }
        #endregion

        #region 將購物車商品加入到訂單
        public object AddOrder(List<ShoppingCartViewModel> shoppingCartViewModels, AddOrderViewModel addOrderViewModel)
        {
            using (_dbContext)
            {
                Order order = new Order();
                OrderDetail orderDetail = new OrderDetail();
                order.CreateDate = DateTime.Now;
                order.Description = addOrderViewModel.OrderDescription;
                order.Address = addOrderViewModel.BuyerAddress;
                order.StateId = 1;
                order.PaymentType = int.Parse(addOrderViewModel.PaymentType);
                order.Phone = addOrderViewModel.Phone;
                order.UserId = addOrderViewModel.BuyerId;
                orderDetail.ProductId = shoppingCartViewModels[0].ProductId;
                //直接存取總金額
                orderDetail.Price = addOrderViewModel.VouchersSum;
                orderDetail.Count = shoppingCartViewModels[0].ProductCount;
                orderDetail.Disable = false;
                order.OrderDetails = orderDetail;
                _dbContext.Order.Add(order);
                _dbContext.SaveChanges();
                //將先前session清除
                HttpContext.Session.Remove("ShoppingCart");
                return new
                {
                    ordernumber = order.Id.ToString(),
                    amount = shoppingCartViewModels[0].ProductPrice * shoppingCartViewModels[0].ProductCount,
                    PayMethod = addOrderViewModel.PaymentType == "0" ? "creditcard" : "VACC"
                };
            }
        }

        #endregion

        #region 加入團購結帳頁面
        [Authorize(Roles = "FullUser")]
        public IActionResult checkout()
        {

            return View();
        }
        #endregion

        #region 等待開團商品頁

        //等待開團商品頁
        [Authorize(Roles = "FullUser")]
        public IActionResult WaitingProduct()
        {
            return View();
        }

        #endregion

        #region 已開團商品頁


        //已開團商品頁
        [Authorize(Roles = "FullUser")]
        public IActionResult ReadyProduct()
        {
            return View();
        }
        #endregion

        #region 新增商品
        //新增商品
        [HttpPost]
        [Authorize(Roles = "FullUser")]
        public IActionResult PostProduct(AddProductViewModel product)
        {
            if (product.PicPath == null)
            {
                return BadRequest();
            }


            var targetUser = GetTargetUser();

            #region 建立商品
            var targetProduct = new Product
            {
                Name = product.Name,
                Content = product.Content,
                Category = product.Category,
                Description = product.Description,
                CreateTime = DateTime.Now,
                EndTime = product.EndTime,
                Price = product.Price,
                User = targetUser,
                Total = product.Total
            };
            _productsRepository.Create(targetProduct);
            _productsRepository.SaveChanges();
            #endregion

            //檔案路徑
            var path = _environment.WebRootPath + "/ProductPicture";

            #region 加入圖片


            foreach (var file in product.PicPath)
            {
                if (file != null)
                {
                    var fileName = DateTime.Now.Ticks + file.FileName;
                    using var fs = System.IO.File.Create($"{path}/{fileName}");
                    file.CopyTo(fs);
                    var pPic = new ProductPic()
                    {
                        Product = targetProduct,
                        PicPath = fileName
                    };
                    _dbContext.ProductPics.Add(pPic);
                }
            }
            _dbContext.SaveChanges();


            #endregion
            return RedirectToAction("index", "Home");
        }


        #endregion

        #region 抓取當前使用者
        //抓取當前使用者

        private User GetTargetUser()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _userRepository.Get(a => a.Email == userEmail);
            return targetUser;
        }


        #endregion

        #region 取得當前連線使用者購物車
        public object GetUserShoppingCart()
        {
            var shoppjson = HttpContext.Session.GetString("ShoppingCart");

            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _userRepository.Get(x => x.Email == userEmail);

            if (shoppjson != null)
            {
                var shoppingcarts = JsonConvert.DeserializeObject<List<ProductCheckViewModel>>(shoppjson);
                var result = shoppingcarts.Where(x => x.BuyerPhone != null && x.BuyerName != null && x.BuyerAddress != null);
                return result != null ? shoppingcarts : "使用者資料尚未填寫";
            }
            else
            {
                return "購物車為空";
            }
        }
        #endregion

        #region 尋找賣方商品資料
        public List<ProductViewModel> GetSellerProducts(int id)
        {
            ProductManage product = new ProductManage(_dbContext);
            var result = product.GetSellerProducts(id);

            return result.ToList();
        }
        #endregion


        public IActionResult newebpaytest()
        {
            return View();
        }

        #region 會員輸入優惠碼增加優惠卷方法並且返回新增的優惠卷
        public object AddVoucher(int UserId,string VoucherName)
        {
            ProductManage product = new ProductManage(_dbContext);
            var result = product.AddVoucher(UserId, VoucherName);
            if(result!=null)
            {
                return result;
            }
            else
            {
                return BadRequest();
            }
        }
        #endregion

        #region 抓取該使用者會員優惠卷
        public List<UserVouchersViewModel> GetUserVoucher(int UserId)
        {
            ProductManage product = new ProductManage(_dbContext);
            var result = product.GetUserVoucher(UserId);
            return result;
        }
        #endregion
    }
}
