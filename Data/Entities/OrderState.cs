using System.ComponentModel.DataAnnotations;

namespace Data.Entities
{
    public class OrderState
    {
        [Key]
        public int StateId { get; set; }
        public string State { get; set; }

    }
}