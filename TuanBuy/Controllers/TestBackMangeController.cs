using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TuanBuy.Controllers
{
    public class TestBackMangeController : Controller
    {
        //後台首頁
        public IActionResult Index()
        {
            return View();
        }
        //訂單管理
        public IActionResult OrderManger()
        {
            return View();
        }
        //使用者管理
        public IActionResult UserManger()
        {
            return View();
        }
        //商品管理
        public IActionResult ProductMangedown()
        {
            return View();
        }
        //優惠眷
        public IActionResult BackService()
        {
            return View();
        }
        
    }
}
