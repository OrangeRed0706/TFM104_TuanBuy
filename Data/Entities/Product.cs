using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    [Table("Product")]

    public class Product
    {
        //商品ID
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        //商品建立時間
        public DateTime CreateTime { get; set; } = DateTime.Now;
        //商品結束時間
        public DateTime EndTime { get; set; } = DateTime.Now.AddDays(10);
        //商品名稱

        [Required]
        public string Name { get; set; }
        //商品描述
        [Required]
        public string Description { get; set; }
        //商品內容
        public string Content { get; set; }
        //商品價格
        [Required]
        public decimal Price { get; set; }
        //商品達標金額

        public decimal Total { get; set; }
        //商品類別
        [Required]
        public string Category { get; set; }
        //商品軟刪除
        public bool Disable { get; set; } = false;
        //關連到UserId
        [ForeignKey("User")]
        public int UserId { get; set; }

        public virtual User User { get; set; }
        //一個商品有多張圖片
        public virtual ICollection<ProductPic> ProductPics { get; set; }
        //一個商品有多則訊息
        public virtual ICollection<ProductMessage> ProductMessage { get; set; }
        //一個商品有多則訂單明細
        public virtual ICollection<OrderDetail> OrderDetails { get; set; }
    }
}