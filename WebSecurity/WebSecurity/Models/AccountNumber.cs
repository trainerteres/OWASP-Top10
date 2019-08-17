using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebSecurity.Models
{
    public class AccountNumber
    {
        public int AccountNumberId { get; set; }

        public int Number { get; set; }

        public string Name { get; set; }

        public DateTime DateOfBirth { get; set; }

        public decimal Balance { get; set; }
    }
}