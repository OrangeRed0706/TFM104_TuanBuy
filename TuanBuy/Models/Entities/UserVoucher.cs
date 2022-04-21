using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace TuanBuy.Models.Entities
{
    public class UserVoucher
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("User")]
        public int MemberId { get; set; }

        [ForeignKey("Voucher")]
        public Guid VoucherId { get; set; }

        public virtual User User { get; set; }

        public virtual Voucher Voucher { get; set; }

    }
}
