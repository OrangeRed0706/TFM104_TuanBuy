using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Business.IServices;
using Data;
using Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Math.EC.Rfc7748;
using TuanBuy.Models;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Extension;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

namespace TuanBuy.Controllers
{

    [Authorize(Roles = "User")]
    public class MemberCenterController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly TuanBuyContext _dbContext;
        private readonly RedisProvider _redisdb;
        private readonly IUserService _userService;
        public MemberCenterController(GenericRepository<User> userRepository, TuanBuyContext dbContext, RedisProvider redisdb, IUserService service)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
            _redisdb = redisdb;
            _userService = service;
        }

        //會員中心首頁
        public IActionResult Index()
        {
            return View();
        }
        //更改密碼頁面
        public IActionResult ResetPassword()
        {
            return View();
        }
        /// <summary>
        /// 通知頁
        /// </summary>
        /// <returns></returns>
        public IActionResult MyNotify()
        {
            return View();
        }

        public List<Notify> GetMyNotify()
        {
            var targetUser = GetTargetUser();
            var a = targetUser.Id;
            var allNotify = _dbContext.UserNotify.Include(X => X.NotifyCategory).Where(x => x.UserId == targetUser.Id && x.Disable == false).OrderByDescending(x => x.CreateDateTime);
            var result = allNotify.Select(x =>
                 new Notify()
                 {
                     NotifyId = x.Id,
                     CreateDateTime = x.CreateDateTime.ToString("G"),
                     Content = x.Content,
                     Sender = x.NotifyCategory.Category
                 }).ToList();

            return result;

        }
        [HttpPost]
        public IActionResult DeleteNotify(int id)
        {
            var cur = _dbContext.UserNotify.FirstOrDefault(x => x.Id == id);
            cur.Disable = true;
            _dbContext.SaveChanges();
            return Ok("刪除成功");
        }
        [HttpPost]
        public IActionResult DeleteAllNotify()
        {
            var user = GetTargetUser();
            var notify = _dbContext.UserNotify.Where(x => x.UserId == user.Id);
            foreach (var userNotify in notify)
            {
                userNotify.Disable = true;
            }

            _dbContext.SaveChanges();

            return Ok("刪除成功");
        }

        public class Notify
        {
            public int NotifyId { get; set; }
            public string CreateDateTime { get; set; }
            public string Sender { get; set; }
            public string Content { get; set; }
        }

        public IActionResult Coupon()
        {
            return View();
        }


        #region 我是買家我購買的商品
        //我是買家我購買的商品
        public IActionResult MyBuyProduct()
        {
            return View();
        }


        #endregion


        //我是賣家我的商品
        public IActionResult MyProduct()
        {
            return View();
        }

        public IActionResult OrderManger()
        {
            return View();
        }



        //接收傳遞過來的URL去解碼判斷啟用會員
        [AllowAnonymous]
        public IActionResult StartMemberState(string s)
        {

            var decodeEmail = Models.AppUtlity.GoDecode.Decode(s);

            var targetUser = _userRepository.Get(x => x.Email == decodeEmail);
            if (_userRepository == null)
            {
                return BadRequest();
            }
            else
            {
                targetUser.State = UserState.普通會員.ToString();
                _userRepository.SaveChanges();
            }

            return RedirectToAction("Login", "Home");
        }

        [AllowAnonymous]
        #region 會員個人販賣商品頁面
        [HttpGet]
        public IActionResult MyStoreSell(int id)
        {
            return View();
        }
        #endregion

        #region 取得賣家商場資料
        [AllowAnonymous]
        [HttpGet]
        public SellerUser GetSellerData(int id)
        {
            var data = new MemberMange(_dbContext);
            var target = data.GetUerData(id);
            return target;
        }
        #endregion

        #region 取得我購買的商品訂單
        public IActionResult GetMyOrder()
        {
            var targetUser = GetTargetUser();

            var data = new OrderManage(_dbContext);
            if (targetUser != null)
            {
                var result = data.GetMyOrder(targetUser.Id);
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }

        }

        #endregion

        #region 取得賣家的訂單
        public IActionResult GetSellerOrder()
        {
            var targetUser = GetTargetUser();

            var data = new OrderManage(_dbContext);
            if (targetUser != null)
            {
                var result = data.GetSellerOrder(targetUser.Id);
                return Ok(result);
            }
            else
            {
                return BadRequest();
            }

        }
        #endregion

        /// <summary>
        /// 出貨
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public IActionResult GoShipping(string id)
        {
            //var targetOrder = _dbContext.Order.FirstOrDefault(o => o.Id == id);
            //if (targetOrder != null) targetOrder.StateId = 3;
            var order = _dbContext.Order
                .Include(order => order.OrderDetails)
                .ThenInclude(x => x.Product)
                .Include(x => x.User);
            //找目標訂單

            var currentOrder = order.FirstOrDefault(x => x.Id == id);

            var sender = _dbContext.User.FirstOrDefault(x => x.Id == currentOrder.OrderDetails.Product.UserId);

            if (currentOrder != null)
            {
                currentOrder.StateId = 3;
                //找賣家ID
                var notifyMessage = "";
                //通知訊息
                notifyMessage = $"您訂購的商品{currentOrder.OrderDetails.Product.Name}已經出貨囉！";

                _dbContext.UserNotify.Add(
                    new UserNotify()
                    {
                        UserId = currentOrder.User.Id,
                        SenderId = sender.Id,
                        Content = notifyMessage,
                        Category = 2
                    });

                _dbContext.SaveChanges();

                var redis3 = _redisdb.GetRedisDb(3);
                var listKey = "Notify_" + currentOrder.User.Id;
                redis3.SaveMessage(listKey, notifyMessage);
            }

            return Ok("訂單狀態已被改變");
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult ForgetPassword([FromBody] ForgetUser user)
        {
            var newPassword = _userService.ForgetPassword(user.Email);
            _userService.SaveChanges();



            var mailbody = $@"<h3>您好這是您的新密碼：</h3>";

            Mail.SendMail(user.Email, "TuanBuy會員忘記密碼", mailbody + newPassword);

            return Ok("成功");
        }




        private User GetTargetUser()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _dbContext.User.FirstOrDefault(x => x.Email == userEmail);
            return targetUser;
        }


    }
}
