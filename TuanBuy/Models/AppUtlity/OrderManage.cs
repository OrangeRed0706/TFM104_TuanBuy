using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MailKit.Search;
using TuanBuy.Controllers;
using TuanBuy.ViewModel;

namespace TuanBuy.Models.Entities
{
    public class OrderManage
    {
        private readonly TuanBuyContext _dbContext;
        public OrderManage(TuanBuyContext context)
        {
            _dbContext = context;
        }
        //產品管理join  
        public List<ProductBackMangeViewModel> GetProduct()
        {
            var product = from products in _dbContext.Product
                          join productpics in _dbContext.ProductPics on products.Id equals productpics.Id
                          join users  in _dbContext.User on products.UserId equals users.Id
                          select new ProductBackMangeViewModel()
                          {
                              PicPath = "/ProductPicture/" + productpics.PicPath,
                              Price = products.Price,
                              ProductId = products.Id,
                              ProductName = products.Name,
                              Category=products.Category,
                              Total=products.Total,
                              CreateTime=products.CreateTime.ToString("g"),
                              EndTime=products.EndTime.ToString("g"),
                              Content=products.Content,
                              Description=products.Description,
                              UserName=users.Name,
                              Disable = products.Disable
                          };
            var result = product.ToList();
            return result;
        }


        //訂單管理join
        public List<OrderBackMangeViewModel> GetOrderDetails()
        {
            var orderdetails = from orderdetail in _dbContext.OrderDetail
                               join oder in _dbContext.Order on orderdetail.OrderId equals oder.Id
                               join product in _dbContext.Product on orderdetail.ProductId equals product.Id
                               join user in _dbContext.User on orderdetail.ProductId equals user.Id
                               where orderdetail.Disable == false
                               select new OrderBackMangeViewModel()
                               {
                                   Address = oder.Address,
                                   Count = orderdetail.Count,
                                   CreateDate = oder.CreateDate.ToString("g"),
                                   OrderId = orderdetail.OrderId,
                                   PaymentType = oder.PaymentType,
                                   Phone = oder.Phone,
                                   Price = orderdetail.Price,
                                   ProductName = product.Name,
                                   UserName = user.Name,
                                   Disable = orderdetail.Disable
                               };
            var result = orderdetails.ToList();
            return result;
        }



        public List<OrderViewModel> GetMyOrder(int id)
        {
            //撈出使用者訂單
            var orders =
                (from order in _dbContext.Order
                 where order.UserId == id
                 join orderDetail in _dbContext.OrderDetail on order.Id equals orderDetail.OrderId
                 join product in _dbContext.Product on orderDetail.ProductId equals product.Id
                 join seller in _dbContext.User on product.UserId equals seller.Id
                 select new { order, product, orderDetail, seller }).ToList();
            //拿使用者訂單去找每個產品的圖片
            var productPics =
                    from myOrderDetail in orders
                    join pic in _dbContext.ProductPics on myOrderDetail.product.Id equals pic.ProductId into pics
                    select new { myOrderDetail, pic = pics.First() };


            var myOrderDetails = new List<OrderViewModel>();

            foreach (var item in productPics)
            {
                var myOrderDetail = new OrderViewModel()
                {
                    OrderId = item.myOrderDetail.order.Id,
                    OrderState = item.myOrderDetail.order.StateId,
                    Description = item.myOrderDetail.order.Description,
                    Count = item.myOrderDetail.orderDetail.Count,
                    PaymentType = item.myOrderDetail.order.PaymentType,
                    Address = item.myOrderDetail.order.Address,
                    SellerId = item.myOrderDetail.seller.Id,
                    SellerName = item.myOrderDetail.seller.Name,
                    ProductName = item.myOrderDetail.product.Name,
                    ProductDescription = item.myOrderDetail.product.Description,
                    ProductPrice = item.myOrderDetail.product.Price,
                    ProductId = item.myOrderDetail.product.Id,
                    OrderPrice = item.myOrderDetail.orderDetail.Price
                };
                if (item.myOrderDetail.product.Id == item.pic.ProductId)
                {
                    myOrderDetail.ProductPath = "/productpicture/" + item.pic.PicPath;
                }
                myOrderDetails.Add(myOrderDetail);
            }
            return myOrderDetails;
        }

        /// <summary>
        /// 撈出會員中心賣家的待出貨商品
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public List<SellerOrderViewModel> GetSellerOrder(int id)
        {
            var result = (
                from user in _dbContext.User
                where user.Id == id
                join product in _dbContext.Product on user.Id equals product.UserId
                where product.Disable == false
                join orderDetail in _dbContext.OrderDetail on product.Id equals orderDetail.ProductId
                join order in _dbContext.Order on orderDetail.OrderId equals order.Id
                orderby order.CreateDate descending
                where order.StateId >= 2
                select new { order, orderDetail, product }).ToList();
            var orderList = new List<SellerOrderViewModel>();

            var buyer =
                (from orders in result
                 join user in _dbContext.User on orders.order.UserId equals user.Id
                 select user).ToList();

            #region 這段不行 莫名其妙
            //foreach (var item in result)
            //{
            //    foreach (var user in buyer)
            //    {
            //        if (user.Id == item.order.UserId)
            //        {
            //            orderList.Add(new SellerOrderViewModel()
            //            {
            //                OrderId = item.order.Id,
            //                OrderDateTime = item.order.CreateDate.ToString("yyyy-MM-dd"),
            //                ProductName = item.product.Name,
            //                Total = item.orderDetail.Count * item.orderDetail.Price,
            //                Address = item.order.Address,
            //                BuyerName = user.Name
            //            });
            //        }
            //    }
            //}
            #endregion
            //OK
            foreach (var item in result)
            {
                var sellerOrder = new SellerOrderViewModel()
                {
                    OrderId = item.order.Id,
                    OrderDateTime = item.order.CreateDate.ToString("yyyy-MM-dd"),
                    ProductName = item.product.Name,
                    Total = item.orderDetail.Count * item.orderDetail.Price,
                    Address = item.order.Address,
                    ProductDescription = item.product.Description,
                    OrderDescription = item.order.Description,
                    OrderState = item.order.StateId,
                    ProductCount = item.orderDetail.Count
                };
                foreach (var user in buyer)
                {
                    if (user.Id == item.order.UserId)
                    {
                        sellerOrder.BuyerName = user.Name;
                    }
                }
                orderList.Add(sellerOrder);
            }

            
            return orderList.OrderBy(x => x.CreateTime).ToList();
        }
    }
}
