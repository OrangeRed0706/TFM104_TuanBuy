using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Org.BouncyCastle.Math.EC.Rfc7748;
using TuanBuy.Models;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

namespace TuanBuy.Controllers
{

    [Authorize(Roles = "User")]
    public class MemberCenterController : Controller
    {
        private readonly IRepository<User> _userRepository;
        private readonly TuanBuyContext _dbContext;
        public MemberCenterController(GenericRepository<User> userRepository,TuanBuyContext dbContext)
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
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

        public IActionResult MyNotify()
        {
            return View();
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
            var target =data.GetUerData(id);
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
            var targetOrder = _dbContext.Order.FirstOrDefault(o => o.Id == id);
            if (targetOrder != null) targetOrder.StateId = 3;
            _dbContext.SaveChanges();
            return Ok("訂單狀態已被改變");
        }
        private User? GetTargetUser()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _dbContext.User.FirstOrDefault(x => x.Email == userEmail);
            return targetUser;
        }
    }
}
