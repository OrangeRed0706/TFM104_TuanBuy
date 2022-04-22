using Business.IServices;
using Data;
using Data.Entities;

namespace Business.Services
{
    public class UserService:IUserService
    {
        private readonly TuanBuyContext _dbContext;

        public UserService(TuanBuyContext dbContext)
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

        public void SaveChanges()
        {
            _dbContext.SaveChanges();
        }
    }
}