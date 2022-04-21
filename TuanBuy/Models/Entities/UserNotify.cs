using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using MailKit.Net.Pop3;

namespace TuanBuy.Models.Entities
{
    public class UserNotify
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public DateTime CreateDateTime { get; set; } = DateTime.Now;
        public int SenderId { get; set; }
        public string Content { get; set; }
        public bool Disable { get; set; } = false;
        [ForeignKey("User")]
        public int UserId { get; set; }
        public virtual User User { get; set; }
        [ForeignKey("NotifyCategory")]
        public int Category { get; set; }
        public virtual NotifyCategory NotifyCategory { get; set; }

    }
}