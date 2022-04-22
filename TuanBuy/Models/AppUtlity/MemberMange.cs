using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Data;
using TuanBuy.Models.Entities;
using TuanBuy.ViewModel;

namespace TuanBuy.Models.AppUtlity
{
    public class MemberMange
    {
        private TuanBuyContext _dbContext;
        public MemberMange(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }

        public SellerUser GetUerData(int userId)
        {
            var first = _dbContext.User.First(u => u.Id == userId);
            var target = new SellerUser()
            {
                Name = first.Name,
                PicPath = "/MemberPicture/" + first.PicPath
            };

            return target;
        }
    }
}
