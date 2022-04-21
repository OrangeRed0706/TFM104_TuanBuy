using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

#nullable disable

namespace TuanBuy.Models.Entities
{
    [Table("Order")]
    public  class Order
    {
        [Required]
        [Key]
        //訂單ID
        public string Id { get; set; } = GenerateStringID();

        //訂單建立時間 
        public DateTime CreateDate { get; set; } = DateTime.Now;
        //訂單軟刪除
        public bool Disable { get; set; } = false;
        //訂單備註

        public string Description { get; set; }
        //訂單地址
        public string Address { get; set; }
        //付款種類
        public int PaymentType { get; set; }
        //訂單連絡電話
        public string Phone { get; set; }

        //關連到使用者ID

        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        //一筆訂單有很多訂單明細
        public virtual OrderDetail OrderDetails { get; set; }

        //訂單的付款狀態
        [ForeignKey("OrderState")]
        public int StateId { get; set; }
        public virtual OrderState OrderState { get; set; }

        private static string GenerateStringID()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int)b + 1);
            }
            return string.Format("TuanBuy_"+"{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
