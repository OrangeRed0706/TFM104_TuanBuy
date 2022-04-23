using Data.Entities;

namespace Business.IServices
{
    public interface IUserService : IDataService<User>
    {
        public string ForgetPassword(string email);

    }
}