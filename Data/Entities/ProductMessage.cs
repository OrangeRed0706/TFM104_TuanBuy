using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class ProductMessage
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.Now;
        public int UserId { get; set; }
        public string MessageContent { get; set; }
        public int ProductId { get; set; }
        public virtual Product Product { get; set; }
    }
}