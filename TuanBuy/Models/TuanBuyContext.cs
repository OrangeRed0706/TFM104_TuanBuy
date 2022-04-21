using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
#nullable disable

namespace TuanBuy.Models.Entities
{
    public partial class TuanBuyContext : DbContext
    {

        public TuanBuyContext(DbContextOptions<TuanBuyContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Order> Order { get; set; }
        public virtual DbSet<OrderDetail> OrderDetail { get; set; }
        public virtual DbSet<Product> Product { get; set; }
        public virtual DbSet<User> User { get; set; }

        public virtual DbSet<ProductMessage> ProductMessages { get; set; }

        public virtual DbSet<ProductPic> ProductPics { get; set; }
        public virtual DbSet<ChatRoom> ChatRooms { get; set; }

        public virtual DbSet<ChatRoomMember> Member_Chats { get; set; }

        public virtual DbSet<ChatMessages> ChatMessages { get; set; }

        public virtual DbSet<ProductSellerReply> ProductSellerReplies { get; set; }

        public virtual DbSet<OrderState> OrderState { get; set; }

        public virtual DbSet<LineMember> LineMember { get; set; }

        public virtual DbSet<Voucher> Vouchers { get; set; }

        public virtual DbSet<UserVoucher> UserVouchers { get; set; }
        public virtual DbSet<UserNotify> UserNotify { get; set; }
        public virtual DbSet<NotifyCategory> NotifyCategory { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ChatRoomMember>().ToTable("Member_Chats");
            modelBuilder.Entity<ChatRoomMember>().HasKey(s => new { s.MemberId, s.ChatRoomId });
            //折扣卷多對多
            modelBuilder.Entity<UserVoucher>().HasKey(s => new { s.MemberId, s.VoucherId });
            //商品和訂單多對多產生訂單明細
            //modelBuilder.Entity<OrderDetail>().HasKey(s => new { s.Order.Id, s.OrderId });

            //modelBuilder.Entity<OrderDetail>().HasOne(X => X.Order).WithOne().OnDelete(DeleteBehavior.ClientNoAction);


            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithOne(o => o.OrderDetails)
                .OnDelete(DeleteBehavior.ClientNoAction);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(pd => pd.Product)
                .WithMany(pd => pd.OrderDetails)
                .HasForeignKey(pt => pt.ProductId)
                .OnDelete(DeleteBehavior.ClientNoAction);

            #region 假資料去FakeData資料夾裡面的json檔案新增
            //使用者資料
            var userJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/FakeData/UserData.json", Encoding.UTF8);
            IList<User> users = JsonConvert.DeserializeObject<IList<User>>(userJsonData);
            modelBuilder.Entity<User>().HasData(users);

            //訂單狀態資料
            var oderStateJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/FakeData/OrderStateData.json", Encoding.UTF8);
            IList<OrderState> oderState = JsonConvert.DeserializeObject<IList<OrderState>>(oderStateJsonData);
            modelBuilder.Entity<OrderState>().HasData(oderState);

            //產品資料
            var productJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/FakeData/ProductData.json", Encoding.UTF8);
            IList<Product> products = JsonConvert.DeserializeObject<IList<Product>>(productJsonData);
            modelBuilder.Entity<Product>().HasData(products);

            //產品圖片
            var productPicJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/FakeData/ProductPicpathData.json", Encoding.UTF8);
            IList<ProductPic> productPathetic = JsonConvert.DeserializeObject<IList<ProductPic>>(productPicJsonData);
            modelBuilder.Entity<ProductPic>().HasData(productPathetic);

            //訂單資料
            var orderJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/FakeData/OrderJson.json", Encoding.UTF8);
            IList<Order> Orders = JsonConvert.DeserializeObject<IList<Order>>(orderJsonData);
            modelBuilder.Entity<Order>().HasData(Orders);

            //訂單明細資料
            var orderDetailJsonData = File.ReadAllText(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + @"/FakeData/OrderDetailJson.json", Encoding.UTF8);
            IList<OrderDetail> orderDetail = JsonConvert.DeserializeObject<IList<OrderDetail>>(orderDetailJsonData);
            modelBuilder.Entity<OrderDetail>().HasData(orderDetail);


            #endregion




            base.OnModelCreating(modelBuilder);
        }
    }
}
