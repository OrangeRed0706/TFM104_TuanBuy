﻿using System;
using System.Collections.Generic;
using System.Linq;
using Data;
using Data.Entities;
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
            var totalSales = (_dbcontext.OrderDetail.ToList().GroupJoin(_dbcontext.Product,
                 ord => ord.ProductId,
                 prd => prd.Id,
                 (ord, prd) => new { ord, prd }
                ).ToList().GroupJoin(_dbcontext.Order,
                 orderdetil => orderdetil.ord.Order.Id,
                 ord => ord.OrderDetails.OrderId,
                 (orderdetil, ord) => new { orderdetil, ord }
                ).Where(x => x.orderdetil.ord.Order.StateId == 3).ToList().Sum(x =>
                        x.orderdetil.prd.Sum(y => y.Price * x.orderdetil.ord.Count)
                ));
            var hotProduct = _dbcontext.Product.ToList().GroupJoin(_dbcontext.OrderDetail,
                   prd => prd.Id,
                   order => order.ProductId,
                   (product, order) => new { product, order }
               ).Select(x => new HotProduct
               {
                   HotProductName = x.product.Name,
                   HotProductCount = x.order.Sum(y => y.Count)
               }).OrderByDescending(x => x.HotProductCount).Take(3).ToList<HotProduct>();
            //團主銷售排行
            var SellerRankingResult = _dbcontext.Order.ToList().GroupJoin(_dbcontext.OrderDetail,
                    ord => ord.Id,
                    orddetail => orddetail.OrderId,
                    (ord, orddetail) => new { Ord = ord, detail = orddetail }
                ).Where(x => x.Ord.StateId == 3).ToList().GroupJoin(_dbcontext.Product,
                    ord => ord.Ord.OrderDetails.ProductId,
                    prd => prd.Id,
                    (ord, prd) => new { ord.detail, ord.Ord }
                ).ToList().SelectMany(
                   ord => ord.detail,
                   (ord, name) => new { ord, name }
                ).ToList().GroupJoin(_dbcontext.User,
                  ord => ord.name.Product.UserId,
                  user => user.Id,
                  (ord, user) => new { ord, user }
                ).GroupBy(x=>x.ord.name.Product.UserId).Select(x => new SellerRanking
                {
                    SellerId = x.Key,
                    PicPath = "/MemberPicture/" + x.FirstOrDefault(y => y.ord.name.Product.User.Id == x.Key).ord.name.Product.User.PicPath,
                    SellerName = x.FirstOrDefault(y=>y.ord.name.Product.User.Id==x.Key).ord.name.Product.User.Name,
                    Price =  x.Sum(y=>y.ord.name.Count * y.ord.name.Product.Price),
                }).OrderByDescending(x => x.Price).Take(3).ToList<SellerRanking>();
            //圖表資訊
            var GraphResult = _dbcontext.Order.ToList().GroupJoin(_dbcontext.OrderDetail,
                    ord => ord.Id,
                    orddetail => orddetail.OrderId,
                    (ord, orddetail) => new { Ord = ord, detail = orddetail }
                ).Where(x => x.Ord.StateId == 3).ToList().GroupJoin(_dbcontext.Product,
                    ord => ord.Ord.OrderDetails.ProductId,
                    prd => prd.Id,
                    (ord, prd) => new { ord.detail, ord.Ord }
                ).ToList().SelectMany(
                    ordResult => ordResult.detail,
                    (ordResult, result) => new { ordResult, result }
                ).GroupBy(x => new { x.ordResult.Ord.CreateDate.Year, x.ordResult.Ord.CreateDate.Month }).Select(x => new Graph
                {
                    Year = x.Key.Year,
                    Month = x.Key.Month,
                    MonthPrice = x.Sum(y=>y.ordResult.Ord.OrderDetails.Count * y.ordResult.Ord.OrderDetails.Product.Price)
                }).OrderByDescending(x=>x.Year).ThenBy(x=>x.Month).ToList<Graph>();

            //.GroupBy(x => x.ordResult.Ord.CreateDate.Month).Select(x=>new Graph { 
            //   Month = x.Key,
            //   MonthPrice = x.Sum(y=>y.ordResult.Ord.OrderDetails.Count * y.ordResult.Ord.OrderDetails.Product.Price)  
            //});

            //.Select(x => new SellerRanking
            // {
            //     PicPath = x.FirstOrDefault(y => y.x.ord.prd.SellerId == x.Key).x.ord.prd.PicPath,
            //     SellerId = x.Key,
            //     SellerName = x.FirstOrDefault(y => y.x.ord.prd.SellerId == x.Key).x.ord.prd.SellerName,
            //     Price = x.Sum(x => x.x.name.OrderDetails.Count * x.x.name.OrderDetails.Product.Price)
            // }).ToList().OrderByDescending(x => x.Price).Take(3);
            ////var SellerRankingResult = SellerRanking.Select(x => x).ToList().GroupBy(y => y.SellerId).Select(x => new SellerRanking
            //{
            //    SellerId = x.Key,
            //    SellerName = x.FirstOrDefault(y => y.SellerId == x.Key).SellerName,
            //    PicPath = x.FirstOrDefault(y => y.SellerId == x.Key).PicPath,
            //    Price = x.Sum(x => Convert.ToInt32(x.Price))
            //}).ToList();
            HomeBackMangeViewModel homeBackMangeViewModel = new HomeBackMangeViewModel()
            {
                UserCount = usercount,
                ProductCount = productCount,
                ProcessOrder = processOrder,
                FinishOrder = finishOrder,
                TotalSales = totalSales,
                //HotproductCount = Convert.ToInt32(hotProduct),
                //ProductName = productName.ToString()
                hotProducts = hotProduct,
                sellerRankings = SellerRankingResult,
                Graphs = GraphResult,
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
                //users.ForEach(x =>
                //{
                //    var cur = listKey + x.Id;
                //    redis3.SaveMessage(cur, notifyMessage);

                //    //這邊只是我想看存進去的東西
                //    var a = new string[redis3.ListLength(cur)];
                //    for (int i = 0; i < (redis3.ListLength(cur)); i++)
                //    {
                //        a[i] = (string.Concat(redis3.ListRange(cur, i)));
                //    }
                //    test.Add(a);
                //});
                




                #endregion

                
                
                return Ok();
            }
        }

        #endregion
        #region//優惠眷管理
        //撈出優惠卷資料
        public List<Voucher> Counpons()
        {
            var counpons = from c in _dbcontext.Vouchers
                           select new Voucher
                           {
                               DiscountDescribe = c.DiscountDescribe,
                               VoucherName = c.VoucherName,
                               VoucherDescribe = c.VoucherDescribe,
                               VouchersAvlAmount = c.VouchersAvlAmount,
                               VouchersDiscount = c.VouchersDiscount
                           };
            var result = counpons.ToList();
            return result;

        }
        //更新優惠卷資料
        [HttpPut]
        public IActionResult UpdateCounpons([FromBody] Voucher Voucher)
        {
            var targetCounpons = _dbcontext.Vouchers.FirstOrDefault(x => x.VoucherName == x.VoucherName);
            if (targetCounpons == null) return BadRequest();
            targetCounpons.VoucherName = Voucher.VoucherName;
            targetCounpons.VoucherDescribe = Voucher.VoucherDescribe;
            targetCounpons.VouchersAvlAmount = Voucher.VouchersAvlAmount;
            targetCounpons.VouchersDiscount = Voucher.VouchersDiscount;
            _dbcontext.SaveChanges();
            return Ok();
        }
        //刪除優惠眷
        [HttpDelete]
        public IActionResult DeleteCounpons(string id)
        {
            var user = _dbcontext.Vouchers.FirstOrDefault(x => x.VoucherName == id);
            _dbcontext.Vouchers.Remove(user);
            _dbcontext.SaveChanges();
            return Ok();
        }
        #endregion
    }
}
