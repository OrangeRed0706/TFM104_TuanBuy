using System.ComponentModel.DataAnnotations;

namespace TuanBuy.Models.Entities
{
    public class NotifyCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}