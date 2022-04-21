using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TuanBuy.Models.AppUtlity;
using TuanBuy.ViewModel;

namespace TuanBuy.Models.Entities
{
    public class ProductManage
    {
        private TuanBuyContext _dbContext;
        public ProductManage(TuanBuyContext dbContext)
        {
            _dbContext = dbContext;
        }
        #region 取得商品頁資料
        public DemoProductViewModel GetDemoProductData(int ProductId)
        {
            //群組join
            var result =
                _dbContext.Product.ToList()
                    //GroupJoin商品圖片
                    .GroupJoin(
                        _dbContext.ProductPics.ToList(),
                        prd => prd.Id,
                        pit => pit.ProductId,
                        (product, productimg) => new {product})
                    .ToList()
                    .Where(x => x.product.Id == ProductId)
                    //GroupJoin使用者
                    .GroupJoin(
                        _dbContext.User,
                        prd => prd.product.UserId,
                        user => user.Id,
                        (prd, user) => new {prd})
                    .ToList()
                    //GroupJoin訂單ID
                    .GroupJoin(
                        _dbContext.OrderDetail,
                        user => user.prd.product.Id,
                        //order => order.Product,
                        orderDetail => orderDetail.ProductId,
                        (user, order) => new {user});
            //目前最熱門的三筆商品
            var hotProduct = _dbContext.Product.ToList().GroupJoin(_dbContext.OrderDetail,
                    prd => prd.Id,
                    order => order.ProductId,
                    (product, order) => new { product, order }
                ).Select(x => new HotProductViewModel
                {
                    HotProductPrice = x.order.Sum(y => y.Count * x.product.Price),
                    HotProductDescribe = x.product.Description,
                    //前端方便轉址用直接帶href
                    HotProducthref = "/Product/DemoProduct/" + x.product.Id,
                    HotProductSeller = x.product.User.Name,
                    HotProductSellerhref = "/MemberCenter/mystoresell/"+x.product.User.Id,
                    HotProductTitle = x.product.Name,
                    HotProductLastTime = x.product.EndTime.Subtract(x.product.CreateTime).Duration().Days.ToString()
                }
                ).OrderByDescending(x=>x.HotProductPrice).Take(3);
            DemoProductViewModel demoProductViewModel = new DemoProductViewModel();
            List<string> productPicPath = new List<string>();
            List<HotProductViewModel> hotProductViewModels = new List<HotProductViewModel>();
            foreach (var item in result)
            {
                
                demoProductViewModel.Id = item.user.prd.product.Id;
                demoProductViewModel.ProductTitle = item.user.prd.product.Name;
                demoProductViewModel.ProductSummary = item.user.prd.product.Content;
                demoProductViewModel.ProductCategory = item.user.prd.product.Category;
                demoProductViewModel.ProductDescription = item.user.prd.product.Description;
                demoProductViewModel.ProductTargetPrice = item.user.prd.product.Total;
                demoProductViewModel.ProductPrice = item.user.prd.product.Price;
                demoProductViewModel.ProductStartTime = item.user.prd.product.CreateTime;
                demoProductViewModel.ProductEndTime = item.user.prd.product.EndTime;
                //開團日期及結束日期差計算
                TimeSpan timeSpan = item.user.prd.product.EndTime.Subtract(item.user.prd.product.CreateTime).Duration();
                demoProductViewModel.ProductLastTime = timeSpan.Days.ToString() + "天" + timeSpan.Hours.ToString() + "小時" + timeSpan.Minutes.ToString() + "分鐘";
                //賣家開團主相關資訊
                demoProductViewModel.SellerId = item.user.prd.product.User.Id;
                demoProductViewModel.Seller = item.user.prd.product.User.NickName;
                //目前團購已訂購人數
                if (item.user.prd.product.OrderDetails != null)
                {
                    demoProductViewModel.Buyers = item.user.prd.product.OrderDetails.Count.ToString();
                    //將目前團購累計金額傳入
                    demoProductViewModel.BuyersSumPrice = item.user.prd.product.OrderDetails.Sum(x => x.Count * item.user.prd.product.Price);
                    //透過目前團購金額在後端進行進度條及金額百分比計算
                    var i = demoProductViewModel.BuyersSumPrice;
                    if (i != 0 && item.user.prd.product.Total != 0)
                    {
                        var a = (i / item.user.prd.product.Total) * 100;
                        if (a >= 100)
                        {
                            a = 100;
                        }
                        demoProductViewModel.Color = GetBarColor.GetColor(a);
                        demoProductViewModel.Percentage = a + "%";
                    }
                    else
                    {
                        demoProductViewModel.Percentage = "0%";

                    }
                    if (item.user.prd.product.Total == 0)
                    {
                        demoProductViewModel.Percentage = "100%";
                    }
                }
                if (item.user.prd.product.ProductPics != null)
                {
                    foreach (var picpath in item.user.prd.product.ProductPics)
                    {
                        productPicPath.Add($"/ProductPicture/{picpath.PicPath}");
                    }
                }
                //最熱門的前三筆商品
                foreach(var hot in hotProduct)
                {
                    hotProductViewModels.Add(hot);
                }

                demoProductViewModel.ProductPicPath = productPicPath;
                demoProductViewModel.HotProducts = hotProductViewModels;
            }
            return demoProductViewModel;
        }
        #endregion

        #region 取得商品頁留言
        public List<ProductMessageViewModel> GetProductMessageData(int ProductId)
        {
            //取得商品頁留言訊息
            var message = (from productmessage in _dbContext.ProductMessages
                           join user in _dbContext.User on productmessage.UserId equals user.Id
                           where productmessage.ProductId == ProductId
                           select new { productmessage, user }).ToList().OrderByDescending(x => x.productmessage.Id);
            //取得賣家回覆訊息
            var sellerReplies = from seller in _dbContext.ProductSellerReplies
                                join user in _dbContext.User on seller.UserId equals user.Id
                                where (message.Select(x => x.productmessage.Id).Contains(seller.ProductMessageId))
                                select new { seller, user };
            //var result = from message in _dbContext.ProductMessages select message;
            List<ProductMessageViewModel> productMessageViewModels = new List<ProductMessageViewModel>();
            //先分裝買家留言訊息 裡面判斷賣家是否有回覆訊息
            foreach (var item in message)
            {
                ProductMessageViewModel productMessage = new ProductMessageViewModel();
                productMessage.MessagePicPaht = item.user.PicPath;
                productMessage.MessageName = item.user.NickName;
                productMessage.MessageContent = item.productmessage.MessageContent;
                productMessage.MessageDateTime = item.productmessage.CreatedDate;
                productMessage.MessageId = item.productmessage.Id;
                //賣家回覆訊息
                foreach (var sellerReply in sellerReplies)
                {
                    if (sellerReply.seller.ProductMessageId == item.productmessage.Id)
                    {
                        productMessage.SellerReply = sellerReply.seller.MessageContent;
                        productMessage.SellerName = sellerReply.user.NickName;
                        productMessage.ReplyDateTime = sellerReply.seller.CreatedDate;
                    }
                }
                productMessageViewModels.Add(productMessage);
            }


            return productMessageViewModels;
        }
        #endregion

        #region 新增商品頁留言 
        public void AddProductMessage(int ProductId, int UserId, string MessageContent)
        {
            using (_dbContext)
            {
                ProductMessage productMessage = new ProductMessage();
                productMessage.ProductId = ProductId;
                productMessage.UserId = UserId;
                productMessage.MessageContent = MessageContent;
                productMessage.CreatedDate = DateTime.Now;
                _dbContext.ProductMessages.Add(productMessage);
                _dbContext.SaveChanges();
            }
        }
        #endregion

        #region 賣家回覆商品頁留言
        public void AddSellerMessage(int ProductMessageId, int SellerId, string MessageContent)
        {
            using (_dbContext)
            {
                ProductSellerReply productSellerReply = new ProductSellerReply();
                productSellerReply.UserId = SellerId;
                productSellerReply.CreatedDate = DateTime.Now;
                productSellerReply.MessageContent = MessageContent;
                productSellerReply.ProductMessageId = ProductMessageId;
                _dbContext.ProductSellerReplies.Add(productSellerReply);
                _dbContext.SaveChanges();
            }
        }
        #endregion

        #region 取得所有商品資料給予進度條剩餘時間等
        public IEnumerable<ProductViewModel> GetProducts()
        {

            var products = GetAllProducts().OrderByDescending(x => x.Id);
            var orderDetails =
                (from orderDetail in _dbContext.OrderDetail
                 where (products.Select(x => x.Id)).Contains(orderDetail.ProductId)
                 select new { orderdetail = orderDetail }).ToList();
            var result = new List<ProductViewModel>();
            foreach (var p in products)
            {
                var i = new ProductViewModel();
                i.Id = p.Id;
                i.Name = p.Name;
                i.Description = p.Description;
                i.Content = p.Content;
                i.Category = p.Category;
                i.PicPath = p.PicPath;
                TimeSpan timeSpan = p.EndTime.Subtract(DateTime.Now).Duration();
                i.LastTime = timeSpan.Days + "天";
                i.Price = p.Price;
                i.Total = 0;
                i.Href = p.Href;
                i.TargetPrice = p.TargetPrice;
                i.Color = "#3366a9";
                foreach (var orderDetail in orderDetails)
                {
                    if (orderDetail.orderdetail.ProductId == p.Id)
                    {
                        //計算該商品目前被購買的總金額
                        i.Total += orderDetail.orderdetail.Count * p.Price;
                    }
                }

                if (i.Total != null && i.Total != 0 && i.TargetPrice != 0)
                {
                    //商品的總金額除以商品的集資金額*100算進度百分比
                    var a = (i.Total / i.TargetPrice) * 100;
                    if (a >= 100)
                    {
                        a = 100;
                    }
                    //依進度條百分比取得顏色
                    i.Color = GetBarColor.GetColor(a);
                    i.Percentage = a + "%";
                }
                else
                {
                    i.Percentage = "0%";

                }
                if (i.TargetPrice == 0)
                {
                    i.Percentage = "100%";
                }
                result.Add(i);
            }

            return result;

        }

        #endregion

        #region 取得單一賣家商品資料
        public IEnumerable<ProductViewModel> GetSellerProducts(int sellerId)
        {

            var products = GetAllProducts().OrderByDescending(x => x.Id).ToList();
            var orderDetails =
                (from orderDetail in _dbContext.OrderDetail
                 where (products.Select(x => x.Id)).Contains(orderDetail.ProductId)
                 select new { orderdetail = orderDetail }).ToList();
            var result = new List<ProductViewModel>();
            foreach (var p in products)
            {
                if (p.UserId == sellerId)
                {
                    var i = new ProductViewModel
                    {
                        Id = p.Id,
                        Name = p.Name,
                        Description = p.Description,
                        Content = p.Content,
                        Category = p.Category,
                        PicPath = p.PicPath
                    };
                    TimeSpan timeSpan = p.EndTime.Subtract(DateTime.Now).Duration();
                    i.LastTime = timeSpan.Days + "天";
                    i.Price = p.Price;
                    i.Total = 0;
                    i.Href = p.Href;
                    i.TargetPrice = p.TargetPrice;
                    i.Color = "#3366a9";
                    foreach (var orderDetail in orderDetails)
                    {
                        if (orderDetail.orderdetail.ProductId == p.Id)
                        {
                            i.Total += orderDetail.orderdetail.Count * p.Price;
                        }
                    }

                    if (i.Total != null && i.Total != 0 && i.TargetPrice != 0)
                    {
                        var a = (i.Total / i.TargetPrice) * 100;
                        if (a >= 100)
                        {
                            a = 100;
                        }
                        i.Color = GetBarColor.GetColor(a);
                        i.Percentage = a + "%";
                    }
                    else
                    {
                        i.Percentage = "0%";

                    }
                    if (i.TargetPrice == 0)
                    {
                        i.Percentage = "100%";
                    }
                    result.Add(i);
                }
            }
            return result;
        }



        #endregion

        #region 取得所有商品資料（單一圖片）

        private List<ProductViewModel> GetAllProducts()
        {
            var product = _dbContext.Product.ToList().GroupJoin(
                _dbContext.ProductPics.ToList(),
                product => product,
                productPic => productPic.Product,
                (p, pic) => new ProductViewModel
                {
                    UserId = p.UserId,
                    User = p.User,
                    Disable = p.Disable,
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    Content = p.Content,
                    Category = p.Category,
                    PicPath = "/productpicture/" + pic.FirstOrDefault()?.PicPath,
                    EndTime = p.EndTime,
                    Price = p.Price,
                    //目標金額是商品的Total欄位
                    TargetPrice = p.Total,
                    Href = "/Product/DemoProduct/" + p.Id
                }
            ).ToList();
            return product;
        }
        #endregion

        #region 會員輸入優惠碼增加優惠卷方法
        public List<UserVouchersViewModel> AddVoucher(int UserId, string VoucherName)
        {
            using (_dbContext)
            {
                var result = _dbContext.Vouchers.FirstOrDefault(x => x.VoucherName == VoucherName);
                if(result!=null)
                {
                    UserVoucher user = new UserVoucher();                   
                    user.VoucherId = result.Id;
                    user.MemberId = UserId;
                    try
                    {
                        _dbContext.UserVouchers.Add(user);
                        _dbContext.SaveChanges();
                        //返回剛才新增優惠卷資料
                        var userVouchers = GetUserVoucher(UserId);
                        return userVouchers;
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex);
                    }                   
                }
            }
            return null;
        }
        #endregion

        #region 取得使用者優惠卷
        public List<UserVouchersViewModel> GetUserVoucher(int UserId)
        {
            using (_dbContext)
            {
                var userVouchers = from user in _dbContext.UserVouchers
                             join vouccher in _dbContext.Vouchers on user.VoucherId equals vouccher.Id
                             where user.MemberId == UserId
                             select new UserVouchersViewModel() { 
                                 VouchersTitle = vouccher.VoucherName,
                                 VouchersDescribe = vouccher.VoucherDescribe,
                                 VouchersDiscount = vouccher.VouchersDiscount,
                                 VouchersId = vouccher.Id,
                                 VouchersPicPath = vouccher.PicPath,
                                 DiscountDescribe = vouccher.DiscountDescribe,
                                 VouchersAvlAmount = vouccher.VouchersAvlAmount,
                             };
                var result = userVouchers.ToList();
                return result;
            }
        }
        #endregion


    }
}
