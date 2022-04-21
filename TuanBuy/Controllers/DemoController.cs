using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Razor.Language.Intermediate;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.OpenApi.Extensions;
using Newtonsoft.Json;
using Org.BouncyCastle.Asn1.Ocsp;
using StackExchange.Redis;
using Topic.Hubs;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Bank.Extensions;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Extension;
using TuanBuy.ViewModel;
using Order = StackExchange.Redis.Order;

namespace TuanBuy.Controllers
{
    public class DemoController : Controller
    {
        private readonly TuanBuyContext _dbContext;
        private readonly IWebHostEnvironment _environment;
        private static IDistributedCache _distributedCache;
        private readonly RedisProvider _mydb;

        public DemoController(TuanBuyContext context, IWebHostEnvironment environment, IDistributedCache distributedCache, RedisProvider Mydb)
        {
            _dbContext = context;
            _environment = environment;
            _distributedCache = distributedCache;
            _mydb = Mydb;
        }

        public IActionResult SingalR()
        {
            return View();
        }

        public IActionResult RedisList()
        {

            var _IDatabase = _mydb.GetRedisDb(3);
            var listKey = "Notify_"+1;
            _IDatabase.KeyDelete(listKey, CommandFlags.FireAndForget);//delete all item
            _IDatabase.SaveMessage(listKey, "通知內容");
            //存取每一個字元

            //put 8 _IDatabase.ListRightPush(listKey, "ricoisme".Select(x => (RedisValue)x.ToString()).ToArray());
            //抓取這list長度
            //Console.WriteLine(_IDatabase.ListLength(listKey));
            //撈取list前三個字 rico
            //var firstFour = _IDatabase.ListRange(listKey, 0, 3);
            //Console.WriteLine(string.Concat(firstFour));
            ////撈取倒數五個字rcosme
            //var lastFive = _IDatabase.ListRange(listKey, -5);
            //Console.WriteLine(string.Concat(lastFive));
            //移除一個字
            _IDatabase.ListRemove(listKey, "i");  //remove i
            Console.WriteLine(string.Concat(_IDatabase.ListRange(listKey)));

            return Ok();
        }














        #region Enum取DisplayName

        public List<string> TestEnum()
        {
            Season test = new Season();
            var a = GetDisplayNames(test);

            return a;
        }
        public List<string> GetDisplayNames(Enum enm)
        {
            var type = enm.GetType();
            var displayNames = new List<string>();
            var names = Enum.GetNames(type);
            foreach (var name in names)
            {
                var field = type.GetField(name);
                var fds = field.GetCustomAttributes(typeof(DisplayAttribute), true);

                if (fds.Length == 0)
                {
                    displayNames.Add(name);
                }
                foreach (DisplayAttribute fd in fds)
                {
                    displayNames.Add(fd.Name);
                }
            }
            return displayNames;
        }
        enum Season
        {
            [Display(Name = "春")]
            Spring,
            [Display(Name = "夏")]
            Summer,
            [Display(Name = "秋")]
            Autumn,
            [Display(Name = "冬")]
            Winter
        }


        #endregion

        public class Users
        {
            public string Name { get; set; }
            public int Age { get; set; }
        }



        public IActionResult Index()
        {

            var user = new Users() { Name = "小王", Age = 20 };
            var json = JsonConvert.SerializeObject(user);
            var bytes = Encoding.UTF8.GetBytes(json);
            _distributedCache.Set("test", bytes);

            var a = Encoding.UTF8.GetString(_distributedCache.Get("test"));
            var yes = JsonConvert.DeserializeObject<Users>(a);

            var db = _mydb.GetRedisDb(0);
            var ricoID = "1";

            db.HashSet(ricoID, RedisProvider.ToHashEntries(user));

            var c = db.HashGetAll(ricoID);
            var dd = RedisProvider.ConvertFromRedis<Users>(c);
            return View();
        }

        public IActionResult TestFav()
        {
            return View();
        }
        public static Dictionary<String, Object> parse(byte[] json)
        {
            string jsonStr = Encoding.UTF8.GetString(json);
            return JsonConvert.DeserializeObject<Dictionary<String, Object>>(jsonStr);
        }



        #region 暫時用不到

        public object GetRedis()
        {

            string userjsonData = HttpContext.Session.GetString("userData");

            //反序列化成List<SelectListitem>集合物件
            var data = JsonConvert.DeserializeObject<User>(userjsonData);

            return data;
        }
        private User GetTargetUser()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;

            var targetUser = _dbContext.User.FirstOrDefault(a => a.Email == userEmail);


            return targetUser;
        }
        public class SellerOrderViewModel
        {
            //訂單ID
            public Guid OrderId { get; set; }
            //商品名稱
            public string ProductName { get; set; }
            //訂單金額
            public decimal Total { get; set; }
            //訂單日期
            public string OrderDateTime { get; set; }
            //出貨地址
            public string Address { get; set; }
            //買家姓名
            public string BuyerName { get; set; }
        }

        public string DemoUrl()
        {
            string url = _environment.WebRootPath;

            return url;

        }
        //public List<OrderViewModel> GetMyOrder(int id)
        //{
        //    //撈出使用者訂單
        //    var orders =
        //        (from order in _dbContext.Order
        //         where order.UserId == id
        //         join orderDetail in _dbContext.OrderDetail on order.Id equals orderDetail.OrderId
        //         join product in _dbContext.Product on orderDetail.ProductId equals product.Id
        //         join seller in _dbContext.User on product.UserId equals seller.Id
        //         select new { order, product, orderDetail, seller }).ToList();
        //    //拿使用者訂單去找每個產品的圖片
        //    var productPics =
        //            from myOrderDetail in orders
        //            join pic in _dbContext.ProductPics on myOrderDetail.product.Id equals pic.ProductId into pics
        //            select new { myOrderDetail, pic = pics.First() };

        //    var myOrderDetails = new List<OrderViewModel>();

        //    foreach (var item in productPics)
        //    {
        //        var myOrderDetail = new OrderViewModel()
        //        {
        //            OrderId = item.myOrderDetail.order.Id,
        //            OrderState = item.myOrderDetail.order.StateId,
        //            Description = item.myOrderDetail.order.Description,
        //            Count = item.myOrderDetail.orderDetail.Count,
        //            PaymentType = item.myOrderDetail.order.PaymentType,
        //            Address = item.myOrderDetail.order.Address,
        //            SellerId = item.myOrderDetail.seller.Id,
        //            SellerName = item.myOrderDetail.seller.Name,
        //            ProductName = item.myOrderDetail.product.Name,
        //            ProductDescription = item.myOrderDetail.product.Description,
        //            ProductPrice = item.myOrderDetail.product.Price,
        //            ProductId = item.myOrderDetail.product.Id
        //        };
        //        if (item.myOrderDetail.product.Id == item.pic.ProductId)
        //        {
        //            myOrderDetail.ProductPath = item.pic.PicPath;
        //        }
        //        myOrderDetails.Add(myOrderDetail);
        //    }
        //    return myOrderDetails;

        //}


        #endregion

        #region 文字編輯器
        public IActionResult HtmlEditText()      
        {    
            return View();
        }
        [HttpPost]
        public IActionResult HtmlEditText(string TextContent)
        {
            using (_dbContext)
            {
                var result = _dbContext.Product.Single(x => x.Id == 2);
                result.Content = TextContent;
                _dbContext.SaveChanges();
            }
            return View();
        }
        #endregion
        #region 文字編輯器的效果
        public IActionResult TextDemo()
        {
            return View();
        }
        #endregion
    }
}