using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using TuanBuy.Models;
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

        public ProductsController(GenericRepository<Product> productsRepository, IWebHostEnvironment environment,
            GenericRepository<User> userRepository)
        {
            _productsRepository = productsRepository;
            _environment = environment;
            _userRepository = userRepository;
        }

        // GET: api/Products
        [Route("MyProducts")]
        [HttpGet]
        public ActionResult<IEnumerable<ProductViewModel>> GetMyProducts()
        {
            var targetUser = GetTargetUser();
            var byproducts = _productsRepository.GetAll().Where(a => a.User == targetUser && a.Disable == false)
                .ToList();

            return byproducts.Select(p => new ProductViewModel
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Content = p.Content,
                    Category = p.Category,
                    PicPath = "/productpicture/" + p.PicPath,
                    EndTime = p.EndTime,
                    Price = p.Price,
                    Href = "/Product/DemoProduct/" + p.Id
                })
                .ToList();
        }

        // GET: api/Products
        [HttpGet]
        public ActionResult<IEnumerable<ProductViewModel>> GetProducts()
        {
            var product = _productsRepository.GetAll().Where(a => a.Disable == false)
                .OrderByDescending(x => x.Id);
            //.ToList();
            var products = new List<ProductViewModel>();
            return product.Select(p => new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Content = p.Content,
                Category = p.Category,
                PicPath = "/productpicture/" + p.PicPath,
                EndTime = p.EndTime,
                Price = p.Price,
                Href = "/Product/DemoProduct/" + p.Id
            }).ToList();
        }

        // GET: api/Products/5
        [HttpGet("{id}")]
        public ActionResult<ProductViewModel> GetProduct(int id)
        {
            var p = _productsRepository.Get(x => x.Id == id);
            if (p == null) return NotFound();

            return new ProductViewModel
            {
                Id = p.Id,
                Name = p.Name,
                Description = p.Description,
                Content = p.Content,
                Category = p.Category,
                PicPath = "/productpicture/" + p.PicPath,
                EndTime = p.EndTime,
                Price = p.Price,
                Href = "/Product/DemoProduct/" + p.Id
            };
        }


        [HttpPut]
        public IActionResult PutProduct([FromBody]UpDateProductViewModel product)
        {
            var p = _productsRepository.Get(a => a.Id == Convert.ToInt32(product.Id));
            p.Price = product.Price;
            p.Content = product.Content;
            p.Description = product.Description;
            p.EndTime = product.EndTime;
            _productsRepository.SaveChanges();

            return Ok();
        }

        // POST: api/Products
        [HttpPost]
        public IActionResult PostProduct(AddProductViewModel product)
        {
            var path = _environment.WebRootPath + "/ProductPicture";

            var pic = product.PicPath.FirstOrDefault();

            if (pic != null)
            {
                var fileName = DateTime.Now.Ticks + pic.FileName;

                using (var fs = System.IO.File.Create($"{path}/{fileName}"))
                {
                    pic.CopyTo(fs);
                }

                _productsRepository.Create(new Product
                {
                    Name = product.Name,
                    PicPath = fileName,
                    Content = product.Content,
                    Category = product.Category,
                    Description = product.Description,
                    CreateTime = DateTime.Now,
                    EndTime = product.EndTime,
                    Price = product.Price
                });
                _productsRepository.SaveChanges();
                return Ok();
            }

            return BadRequest();
        }

        // DELETE: api/Products/5
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

        private User GetTargetUser()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _userRepository.Get(a => a.Email == userEmail);
            return targetUser;
        }
    }
}