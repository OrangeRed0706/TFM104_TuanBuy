using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TuanBuy.Models.Entities
{
    public class Voucher
    {
        //折價卷ID
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid Id { get; set; }
        //折價卷名稱
        public string VoucherName { get; set; }
        //折價卷敘述
        public string VoucherDescribe { get; set; }
        //折價卷圖片
        public string PicPath { get; set; }
        //折價卷優惠敘述
        public string DiscountDescribe { get; set; }
        //折價卷折數
        public decimal VouchersDiscount { get; set; }
        //折價卷可使用金額
        public decimal VouchersAvlAmount { get; set; }

        public virtual ICollection<UserVoucher> UserVouchers { get; set; }
    }
}
