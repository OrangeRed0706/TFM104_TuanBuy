using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Facebook;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using TuanBuy.Models;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Bank.Extensions;
using TuanBuy.Models.Entities;
using TuanBuy.Service;
using TuanBuy.ViewModel;

namespace TuanBuy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly TuanBuyContext _dbContext;
        public HomeController(ILogger<HomeController> logger, TuanBuyContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }
        //首頁
        public IActionResult Index()
        {
            return View();
        }
        //登入畫面
        public IActionResult Login()
        {
            return View();
        }
        //註冊畫面
        public IActionResult Register()
        {
            return View();
        }

        public IActionResult ForgetPassword()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        public IActionResult FbLogin()
        {
            var p = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("Response")
            };
            return Challenge(p, FacebookDefaults.AuthenticationScheme);
        }

        public IActionResult GoogleLogin()
        {
            var p = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("Response")
            };

            return Challenge(p, GoogleDefaults.AuthenticationScheme);
        }


        public async Task<IActionResult> ResponseAsync()
        {
            Console.WriteLine("進來ㄌ");
            
            var res = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            if (res.Principal == null) return BadRequest();

            var data = res.Principal.Claims.Select(x => new
            {
                x.Type,
                //x.Issuer,
                x.Value,
                //x.OriginalIssuer
            });

            var test = res.Principal.Claims.Select(x => new KeyValuePair<string, string>(x.Type, x.Value)).ToList();
            var email = test.GetValueByKey("emailaddress");
            var surname = test.GetValueByKey("surname");
            var givenname = test.GetValueByKey("givenname");

            var checkUser = _dbContext.User.FirstOrDefault(u => u.Email == email);
            if (checkUser != null)
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, checkUser.NickName),
                    new(ClaimTypes.Email, checkUser.Email),
                    new("Userid", checkUser.Id.ToString()),
                    new("NickName", checkUser.NickName),
                    new("Email", checkUser.Email),
                    new("UserName", checkUser.Name),
                    new("PicPath",checkUser.PicPath),
                };
                //將使用者資訊存入
                //
                var jsonstring = JsonConvert.SerializeObject(new
                {
                    checkUser.Email,
                    checkUser.NickName,
                    checkUser.Id,
                    checkUser.PicPath
                });
                HttpContext.Session.SetString("userData", jsonstring);
                claims.Add(new Claim(ClaimTypes.Role, "HelloMember"));
                if (checkUser.State == "普通會員" | checkUser.State == "正式會員" | checkUser.State == "系統管理員") claims.Add(new Claim(ClaimTypes.Role, "User"));
                if (checkUser.State == "正式會員" | checkUser.State == "系統管理員") { claims.Add(new Claim(ClaimTypes.Role, "FullUser")); }
                if (checkUser.State == "系統管理員") { claims.Add(new Claim(ClaimTypes.Role, "SystemAdmin")); }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);
            }
            else
            {
                var user = new User()
                {
                    Name = surname+givenname,
                    Email = email,
                    Password = GoEncrytion.encrytion(surname + givenname),
                    State = UserState.普通會員.ToString()
                };
                var userMange = new UserMange(_dbContext);
                userMange.CreateOAuthUser(user);

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
                //將使用者資訊存入
                //
                var jsonstring = JsonConvert.SerializeObject(new
                {
                    user.Email,
                    user.NickName,
                    user.Id,
                    user.PicPath
                });
                HttpContext.Session.SetString("userData", jsonstring);
                claims.Add(new Claim(ClaimTypes.Role, "HelloMember"));
                if (user.State == "普通會員" | user.State == "正式會員" | user.State == "系統管理員") claims.Add(new Claim(ClaimTypes.Role, "User"));
                if (user.State == "正式會員" | user.State == "系統管理員") { claims.Add(new Claim(ClaimTypes.Role, "FullUser")); }
                if (user.State == "系統管理員") { claims.Add(new Claim(ClaimTypes.Role, "SystemAdmin")); }

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                await HttpContext.SignInAsync(claimsPrincipal);

            };

            return RedirectToAction("Index");
        }


    }
}

