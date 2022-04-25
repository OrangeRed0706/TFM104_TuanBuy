using System;
using System.Linq;
using System.Net.Mail;
using System.Threading.Channels;
using Business.IServices;
using Business.Services.Utility;
using Data;

namespace Business.Services
{
    public class TaskScheduling : ITaskScheduling
    {
        private TuanBuyContext _dbContext;
        public TaskScheduling(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void Print()
        {
            Console.WriteLine($"HanFire recurring job!");
        }

        public void DailyBirthday()
        {

            var body = $@"<h3>Happy Birthday！</h3>
                <p> 生日快樂！我們幫你準備了生日優惠卷！</p>";
            var getHappyBirthday = _dbContext.User.Where(x => x.Birth == DateTime.Today).ToList();
            getHappyBirthday.ForEach((x) =>
            {
                Mail.SendMail(x.Email,"TuanBuy祝你生日快樂！ 這是您的生日優惠卷", body);
            } );
        }
        public void PullProduct()
        {

            var getHappyBirthday = _dbContext.Product.Where(x => x.EndTime == DateTime.Today).ToList();
            getHappyBirthday.ForEach(x=>x.Disable=true);
            
        }
    }
}