using System.Linq;
using Business.IServices;
using Business.Services.Utility;
using Data;
using Data.Entities;

namespace Business.Services
{
    public class UsersService : IUserService
    {
        private readonly TuanBuyContext _dbContext;

        public UsersService(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }
        public void Add(User instance)
        {
            _dbContext.User.Add(instance);
        }

        public void Update(User instance)
        {
            throw new System.NotImplementedException();
        }

        public void Delete(User instance)
        {
            throw new System.NotImplementedException();
        }

        public string ForgetPassword(string email)
        {
            var curUser = _dbContext.User.FirstOrDefault(x => x.Email == email);
            if (curUser != null)
            {
                return curUser.Password = EncrytionString.encrytion(curUser.Email);
            }
            else throw new System.NotImplementedException();

        }

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}