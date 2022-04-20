using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using StackExchange.Redis;
using TuanBuy.Models.AppUtlity;
using TuanBuy.Models.Entities;
using TuanBuy.Models.Extension;
using TuanBuy.ViewModel;

namespace TuanBuy.Controllers
{
    public class BackstageMangeController : Controller
    {
        private readonly TuanBuyContext _dbcontext;
        private readonly RedisProvider _redisdb;
        public BackstageMangeController(TuanBuyContext context, RedisProvider redisdb)
        {
            _dbcontext = context;
            _redisdb=redisdb;
        }

        #region 會員管理
        public List<UserBackMange> GetUsers()
        {
            var ableUsers = _dbcontext.User.Where(x => x.Disable == false);

            return ableUsers.Select(u => new UserBackMange
            {
                Name = u.Name,
                Email = u.Email,
                State = u.State,
                Birth = u.Birth,
                Phone = u.Phone
            }).ToList();
        }

        //更新使用者資料
        [HttpPut]
        public IActionResult UpdateUser([FromBody] UserBackMange user)
        {
            var targetUser = _dbcontext.User.FirstOrDefault(x => x.Email == user.Email);
            if (targetUser == null) return BadRequest();

            targetUser.Name = user.Name;
            targetUser.Birth = user.Birth;
            targetUser.State = user.State;
            targetUser.Phone = user.Phone;
            _dbcontext.SaveChanges();
            return Ok();
        }
        //軟刪除使用者
        [HttpDelete]
        public IActionResult DeleteUser(string id)
        {
            var user = _dbcontext.User.FirstOrDefault(x => x.Email == id);
            if (user == null) return BadRequest();
            user.Disable = true;
            _dbcontext.SaveChanges();
            return Ok();
        }
        #endregion


        #region 訂單管理
        //秀出OrderManage資訊
        public List<OrderBackMangeViewModel> TestJoin()
        {
            var BackOrder = new OrderManage(_dbcontext);
            var result = BackOrder.GetOrderDetails();
            return result;
        }
        //修改訂單
        [HttpPut]
        public IActionResult UpdateOrder([FromBody] OrderBackMangeViewModel order)
        {

            var orders = _dbcontext.Order.FirstOrDefault(x => x.Id == order.OrderId);
            if (orders == null) return BadRequest();
            orders.Address = order.Address;
            orders.Phone = order.Phone;
            _dbcontext.SaveChanges();
            return Ok();

        }
        //刪除訂單
        [HttpDelete]
        public IActionResult DeleteOrder(string id)
        {
            var user = _dbcontext.OrderDetail.FirstOrDefault(x => x.OrderId == id);
            if (user == null) return BadRequest();
            //user = user.Select(x => new OrderDetail() { Disable = true });
            user.Disable = true;
            _dbcontext.SaveChanges();
            return Ok();
        }
        #endregion
        #region 產品管理
        //撈出所有產品資料
        public List<ProductBackMangeViewModel> ProductJoin()
        {
            var BackOrder = new OrderManage(_dbcontext);
            var result = BackOrder.GetProduct();
            return result;
        }

        //產品下架
        [HttpDelete]
        public IActionResult ProductDown(int id)
        {
            var user = _dbcontext.Product.FirstOrDefault(x => x.Id == id);
            if (user == null) return BadRequest();
            //user = user.Select(x => new OrderDetail() { Disable = true });
            user.Disable = true;
            _dbcontext.SaveChanges();
            return Ok();
        }

        //產品上架
        [HttpDelete]
        public IActionResult ProductUp(int id)
        {
            var user = _dbcontext.Product.FirstOrDefault(x => x.Id == id);
            if (user == null) return BadRequest();
            //user = user.Select(x => new OrderDetail() { Disable = true });
            user.Disable = false;
            _dbcontext.SaveChanges();
            return Ok();
        }
        #endregion

        //後台首頁
        public HomeBackMangeViewModel Homeinformation()
        {
            var usercount = _dbcontext.User.Count();
            var productCount = _dbcontext.Product.Where(x => x.Disable == false).Count();
            var processOrder = _dbcontext.Order.Where(x => x.StateId == 2).Count();
            var finishOrder = _dbcontext.Order.Where(x => x.StateId == 4).Count();
            var totalSales = _dbcontext.OrderDetail.Select(x => x.Price).Sum();
            //var hotProduct = _dbcontext.OrderDetail.GroupJoin(
                
            //    )
            //var hotProduct = _dbcontext.OrderDetail.OrderBy(x => x.Count).Take(3);
            //var productName= hotProduct.Select(x => new { name = x.Product.Name });
            HomeBackMangeViewModel homeBackMangeViewModel = new HomeBackMangeViewModel() 
            {
                UserCount = usercount,
                ProductCount = productCount,
                ProcessOrder = processOrder,
                FinishOrder = finishOrder,
                TotalSales = totalSales,
                //HotproductCount = Convert.ToInt32(hotProduct),
                //ProductName = productName.ToString()
            };
            return homeBackMangeViewModel;
            


        }


        [HttpPost]
        #region 後台新增優惠卷
        public object AddVouchers(UserVouchersViewModel userVouchersViewModel)
        {
            using (_dbcontext)
            {
                Voucher voucher = new Voucher()
                {
                    VoucherName = userVouchersViewModel.VouchersTitle,
                    VoucherDescribe = userVouchersViewModel.VouchersDescribe,
                    DiscountDescribe = userVouchersViewModel.DiscountDescribe,
                    VouchersDiscount = userVouchersViewModel.VouchersDiscount,
                    VouchersAvlAmount = userVouchersViewModel.VouchersAvlAmount
                };
                _dbcontext.Vouchers.Add(voucher);

                #region 新增優惠卷給所有使用者
                var users = _dbcontext.User.ToList();
                var notifyMessage = $"請輸入「{userVouchersViewModel.VouchersTitle}」兌換優惠卷喔";

                var entityEntries = users.Select(x =>
                    _dbcontext.UserNotify.Add(
                        new UserNotify
                        {
                            UserId = x.Id,
                            SenderId = 0,
                            Content = notifyMessage,
                            Category = 1
                        })).ToList();
                _dbcontext.SaveChanges();

                //存進Redis
                var redis3 = _redisdb.GetRedisDb(3);
                var listKey = "Notify_";
                var test =new List<string[]>();
                users.ForEach(x =>
                {
                    var cur = listKey + x.Id;
                    redis3.SaveMessage(cur, notifyMessage);

                    //這邊只是我想看存進去的東西
                    var a = new string[redis3.ListLength(cur)];
                    for (int i = 0; i < (redis3.ListLength(cur)); i++)
                    {
                        a[i] = (string.Concat(redis3.ListRange(cur, i)));
                    }
                    test.Add(a);
                });
                




                #endregion

                
                
                return Ok();
            }
        }

        #endregion
    }
}
