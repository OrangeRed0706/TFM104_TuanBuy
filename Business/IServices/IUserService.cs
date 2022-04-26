using Data.Entities;

namespace Business.IServices
{
    public interface IUserService : IDataService<User>
    {
        public string ForgetPassword(string email);
        bool CreateOAuthUser(User user);
        void CreateTuanButChat(int MemberId);
        User GeUser(string email);
    }
}