using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Data.Entities
{
    public class LineMember
    {
        //自動屬性（抽象屬性寫法）
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public Guid id { set; get; }
        [Required]
        public string userid { set; get; }
        [Required]
        public bool status { set; get; }
        [Required]
        public DateTime timestamp { set; get; }
        [Required]
        public string type { set; get; }
        public DateTime? unfollowdatetime { set; get; }
    }
}