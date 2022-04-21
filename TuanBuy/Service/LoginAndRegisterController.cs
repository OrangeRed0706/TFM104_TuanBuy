using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using TuanBuy.Models;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

namespace TuanBuy.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class LoginAndRegisterController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<User> _userRepository;
        private readonly RedisProvider _redisDb;

        public LoginAndRegisterController(GenericRepository<User> userRepository,
            IHttpContextAccessor httpContextAccessor, RedisProvider redisDb)
        {
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
            _redisDb = redisDb;
        }

        [HttpGet("{email}")]
        //如果Email沒被使用過回傳1，不然回傳0
        public int Check(string email)
        {
            var user = _userRepository.Get(a => a.Email == email);
            if (user == null)
                return 1;
            return 0;
        }

        /// <summary>
        ///     取得UserData
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public string GetUserData()
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var claim = _httpContextAccessor.HttpContext.User.Claims.ToList();
                if (claim.Count == 0) return "null";
                var userName = claim.FirstOrDefault(a => a.Type == "UserName")?.Value;
                if (userName == null)
                {
                    userName = claim.FirstOrDefault(a =>
                        a.Type == "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/name")?.Value;
                }
                //var userName = Claim.Where(a => a.Type == "UserName").First().Value;


                return userName;
            }

            return "null";
        }

        [HttpPost]
        //登入順便給予權限
        public string Login(UserRegister loginUser)
        {
            var user = _userRepository.Get(a => a.Email == loginUser.Email && a.Password == loginUser.Password);
            if (user == null || user.Email == null || loginUser.Email == "" || loginUser.Password == "")
                return "帳號密碼錯誤";

            if (user.State == "未驗證") return "帳號未啟用";

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.NickName),
                new(ClaimTypes.Email, user.Email),
                new("Userid", user.Id.ToString()),
                new("NickName", user.NickName),
                new("Email", user.Email),
                new("UserName", user.Name),
                new("PicPath",user.PicPath),
            };
            //將使用者資訊存入session
            var jsonstring = JsonConvert.SerializeObject(new
            {
                user.Email,
                user.NickName,
                user.Id,
                user.PicPath
            });
            HttpContext.Session.SetString("userData", jsonstring);

            #region 將使用者資訊存入redis

            var db = _redisDb.GetRedisDb(1);
            var redisUser = new UserData()
            {
                Email = user.Email,
                NickName = user.NickName,
                Id = user.Id,
                PicPath = user.PicPath
            };
            db.HashSet(user.Id.ToString(), RedisProvider.ToHashEntries(redisUser));

            #endregion


            claims.Add(new Claim(ClaimTypes.Role, "HelloMember"));
            if (user.State == "普通會員" |user.State == "正式會員" | user.State == "系統管理員") claims.Add(new Claim(ClaimTypes.Role, "User"));
            if (user.State == "正式會員" | user.State == "系統管理員") {claims.Add(new Claim(ClaimTypes.Role, "FullUser"));}
            if (user.State == "系統管理員") {claims.Add(new Claim(ClaimTypes.Role, "SystemAdmin"));}
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
            HttpContext.SignInAsync(claimsPrincipal);
            //HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));
            return "登入成功";
        }

        [HttpDelete]
        public void Logout()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _userRepository.Get(x => x.Email == userEmail);

            #region 移除Redis中的使用者資料

            var db = _redisDb.GetRedisDb(1);
            db.KeyDelete(targetUser.Id.ToString());


            #endregion

            //清除Session
            HttpContext.Session.Remove("userData");

            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        }
    }
}