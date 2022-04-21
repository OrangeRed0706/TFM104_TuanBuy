using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TuanBuy.ViewModel;

#nullable disable

namespace TuanBuy.Models.Entities
{
    [Table("User")]

    public class User
    {
        [Required]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        public string NickName { get; set; } = "暱稱";
        public string Name { get; set; }
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public int Id { get; set; }
        public DateTime? Birth { get; set; }
        public string Phone { get; set; }
        public string BankAccount { get; set; }
        public string State { get; set; } = UserState.未驗證.ToString();
        public int Sex { get; set; } = 1;
        public string Address { get; set; }
        public string PicPath { get; set; } = "637843188933582087init.jpg";
        public bool Disable { get; set; } = false;
        public string Friend { get; set; }
        public virtual ICollection<Order> Order { get; set; }
        public virtual ICollection<Product> Product { get; set; }
        public virtual ICollection<ChatRoomMember> ChatRoom { get; set; }

        public virtual ICollection<UserVoucher> UserVoucher { get; set; }
        public virtual ICollection<UserNotify> UserNotify { get; set; }
    }
}
