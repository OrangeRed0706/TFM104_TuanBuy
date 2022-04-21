using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TuanBuy.Models;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TuanBuy.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class MemberCenterController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IRepository<User> _userRepository;

        public MemberCenterController(GenericRepository<User> userRepository, IHttpContextAccessor httpContextAccessor,
            IWebHostEnvironment environment)
        {
            _httpContextAccessor = httpContextAccessor;
            _userRepository = userRepository;
            _environment = environment;
        }
        // GET: api/<MemberCenterController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{

        //    return new string[] { "value1", "value2" };
        //}

        // GET api/<MemberCenterController>/5

        #region 取得使用者資料
        [HttpGet]
        public UserViewModel Get()
        {
            var targetUser = GetTargetUser();
            if (targetUser == null) return new UserViewModel();
            var userData = new UserViewModel
            {
                Email = targetUser.Email,
                Name = targetUser.Name,
                Phone = targetUser.Phone,
                Address = targetUser.Address,
                Birth = targetUser.Birth,
                Sex = targetUser.Sex,
                BankAccount = targetUser.BankAccount,
                PicPath = "/MemberPicture/" + targetUser.PicPath
            };
            return userData;
        }



        #endregion

        #region 更新使用者資料
        // POST api/<MemberCenterController>
        //更新使用者資料
        [HttpPost]
        public void Post([FromForm] UserUpdate user)
        {
            var targetUser = GetTargetUser();
            var fileName = targetUser.PicPath;
            if (user.PicPath != null)
            {
                var path = _environment.WebRootPath + "/MemberPicture";
                var pic = user.PicPath;
                fileName = DateTime.Now.Ticks + pic.FileName;
                using var fs = System.IO.File.Create($"{path}/{fileName}");
                pic.CopyTo(fs);
            }

            var fullMember = user.Name != "" || user.Phone != "" || user.Address != "" || user.Birth != null;
            if (_httpContextAccessor.HttpContext != null)
            {
                targetUser.Name = user.Name;
                targetUser.Phone = user.Phone;
                targetUser.Birth = user.Birth;
                targetUser.Address = user.Address;
                targetUser.Sex = user.Sex;
                targetUser.PicPath = fileName;
                string userjsonData = HttpContext.Session.GetString("userData");
                if (!String.IsNullOrEmpty(userjsonData))
                {
                    //反序列化成List<SelectListitem>集合物件
                    var data = Newtonsoft.Json.JsonConvert.DeserializeObject<User>(userjsonData);
                    //將檔案名稱丟入session頭貼路徑
                    //將使用者資訊存入session
                    var jsonstring = JsonConvert.SerializeObject(new User
                    {
                        Id = data.Id,
                        Email = data.Email,
                        NickName = user.Name,
                        PicPath = fileName
                    });
                    //清除當前session
                    HttpContext.Session.Remove("userData");
                    //重新設置新session
                    HttpContext.Session.SetString("userData", jsonstring);
                }
                ////將使用者資訊session重新更新;
                //var jsonstring = JsonConvert.SerializeObject(new
                //{
                //    user.Email,
                //    user.NickName,
                //    user.Id,
                //    user.PicPath
                //});
                //HttpContext.Session.SetString("userData", jsonstring);
                if (fullMember)
                {
                    HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                    targetUser.State = UserState.正式會員.ToString();
                    //這段有問題 不能直接更改會員資料
                    var claims = new List<Claim>
                    {
                        new(ClaimTypes.Name, targetUser.NickName),
                        new(ClaimTypes.Email, targetUser.Email),
                        new("Userid", targetUser.Id.ToString()),
                        new("NickName", targetUser.NickName),
                        new("Email", targetUser.Email),
                        new("UserName", targetUser.Name),
                        new("PicPath",targetUser.PicPath),
                    };

                    claims.Add(new Claim(ClaimTypes.Role, "HelloMember"));
                    if (targetUser.State == "普通會員" | targetUser.State == "正式會員" | targetUser.State == "系統管理員") claims.Add(new Claim(ClaimTypes.Role, "User"));
                    if (targetUser.State == "正式會員" | targetUser.State == "系統管理員") { claims.Add(new Claim(ClaimTypes.Role, "FullUser")); }
                    if (targetUser.State == "系統管理員") { claims.Add(new Claim(ClaimTypes.Role, "SystemAdmin")); }
                    var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                    HttpContext.SignInAsync(claimsPrincipal);

                }
            }

            _userRepository.SaveChanges();
        }


        #endregion

        #region 更改使用者密碼
        // 更改使用者密碼
        [HttpPut]
        public void Put([FromBody] UserUpdate user)
        {
            if (_httpContextAccessor.HttpContext != null)
            {
                var claim = _httpContextAccessor.HttpContext.User.Claims.ToList();
                var userMail = claim.First(a => a.Type == "UserEmail").Value;
                var targetUser = _userRepository.Get(a => a.Email == userMail);
                targetUser.Password = user.Password;
            }

            _userRepository.SaveChanges();
        }


        // DELETE api/<MemberCenterController>/5


        #endregion

        #region 刪除使用者

        //軟刪除使用者
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var target = _userRepository.Get(x => x.Id == id);
            target.Disable = true;
            return Ok("使用者刪除成功");
        }

        #endregion

        #region 取得目標使用者
        private User GetTargetUser()
        {
            var claim = HttpContext.User.Claims;
            var userEmail = claim.FirstOrDefault(a => a.Type == ClaimTypes.Email)?.Value;
            var targetUser = _userRepository.Get(a => a.Email == userEmail);
            return targetUser;
        }


        #endregion

    }
}