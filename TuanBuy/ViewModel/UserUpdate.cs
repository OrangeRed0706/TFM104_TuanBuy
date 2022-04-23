using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;

namespace TuanBuy.ViewModel
{
    public class UserUpdate
    {
        public string Password { get; set; }
        public string Name { get; set; }
        public string NickName { get; set; }
        public string Phone { get; set; }
        public string BankAccount { get; set; }
        public DateTime? Birth { get; set; }
        public int Sex { get; set; }
        public string Address { get; set; }
        public IFormFile PicPath { get; set; }

    }
}