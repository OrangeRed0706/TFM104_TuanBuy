using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using Data.Entities;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using TuanBuy.Models;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Interface;
using TuanBuy.ViewModel;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TuanBuy.Service
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IRepository<User> _userRepository;

        public UserController(GenericRepository<User> userRepository)
        {
            _userRepository = userRepository;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<User> Get()
        {
            return _userRepository.GetAll().ToList();
        }

        // GET api/<UserController>/5
        [HttpGet("{id}")]
        public User Get(int id)
        {
            return _userRepository.Get(a => a.Id == id);
        }


        // POST api/<UserController>
        //註冊帳號，寄發驗證信
        [HttpPost]
        public void Post(UserRegister user)
        {
            var userEntity = new User
            {
                Email = user.Email,
                Name = user.Name,
                Password = user.Password
            };
            string completeUrl = Request.GetDisplayUrl().ToString();
            
            //組出環境網址
            var url = new StringBuilder();
            url.Append(Request.Scheme); //https
            url.Append("://");
            url.Append(Request.Host.Value);//"localhost:5001"
            url.Append("/MemberCenter/StartMemberState/?s=");
            var vrCode = GoEncrytion.encrytion(user.Email);
            url.Append(vrCode);

            var body = url.ToString();
            Mail.SendMail(user.Email, "TuanBuy註冊會員，啟動網址", body);
            _userRepository.Create(userEntity);
        }

        // PUT api/<UserController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] UserUpdate userEntity)
        {
            var user = _userRepository.Get(a => a.Id == id);

            user.Name = userEntity.Name;
            //user.Birth = userEntity.Birth;
            user.Phone = userEntity.Phone;
            user.BankAccount = userEntity.BankAccount;

            _userRepository.Update(user);
        }

        // DELETE api/<UserController>/5
        [HttpDelete("{id}")]
        public void Delete(User userEntity)
        {
            _userRepository.Delete(userEntity);
        }
    }
}