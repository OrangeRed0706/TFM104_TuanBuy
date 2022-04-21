using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class NotifyCategory
    {
        [Key]
        public int CategoryId { get; set; }
        public string Category { get; set; }
    }
}