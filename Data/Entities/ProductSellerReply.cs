using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class ProductSellerReply
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public string MessageContent { get; set; }

        [ForeignKey("ProductMessage")]
        public int ProductMessageId { get; set; }
        public virtual ProductMessage ProductMessage { get; set; }
    }
}