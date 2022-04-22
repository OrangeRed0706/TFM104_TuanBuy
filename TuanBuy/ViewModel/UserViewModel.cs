using System;

namespace TuanBuy.ViewModel
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string BankAccount { get; set; }
        public DateTime? Birth { get; set; }
        public int Sex { get; set; }
        public string Address { get; set; }
        public string PicPath { get; set; }
    }

    public class SellerUser
    {
        public string Name { get; set; }
        public string PicPath { get; set; }
    }
    public class UserBackMange
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public string Phone { get; set; }
        public string State { get; set; }
        public DateTime? Birth { get; set; }

    }


    public class UserData
    {
        public string Email { get; set; }
        public string NickName { get; set; }
        public int Id { get; set; }
        public string PicPath { get; set; }
    }

    public class UserVouchersViewModel
    {
        public Guid VouchersId { get; set; }
        //優惠卷圖片
        public string VouchersPicPath { get; set; } = "/ProductPicture/優惠劵圖.jpg";
        //優惠卷標題
        public string VouchersTitle { get; set; }
        //優惠卷描述
        public string VouchersDescribe { get; set; }
        //優惠卷折扣敘述
        public string DiscountDescribe { get; set; }
        //優惠折扣數
        public decimal VouchersDiscount { get; set; }
        //折價卷可使用金額
        public decimal VouchersAvlAmount { get; set; }
        //折價劵訊息狀態
        public string VoucherStateMessage { get; set; }
    }
}
