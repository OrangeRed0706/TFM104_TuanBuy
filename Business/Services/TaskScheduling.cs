using System;
using System.Linq;
using System.Threading.Channels;
using Business.IServices;
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
            Console.WriteLine("現在時間："+DateTime.Now.ToString("d"));
            //var getBirthday = _dbContext.User.Where(x=> x.Birth == )
            //foreach (var birthday in getBirthday)
            //{
            //    Console.WriteLine(birthday.Birth.ToString());
            //}
        }
    }
}