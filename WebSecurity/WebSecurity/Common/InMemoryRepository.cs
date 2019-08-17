using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSecurity.Models;

namespace WebSecurity.Common
{
    public class InMemoryRepository
    {

        public InMemoryRepository()
        {
            AccountNumber = new List<AccountNumber>();

            AccountNumber.Add(new AccountNumber() { AccountNumberId = 1, Name = "Mary Ann", Number = 123456, DateOfBirth = DateTime.Today.AddDays(-13000), Balance = 10000.00m });
            AccountNumber.Add(new AccountNumber() { AccountNumberId = 1, Name = "Bill Gates", Number = 123789, DateOfBirth = DateTime.Today.AddDays(-14000), Balance = 10000000.00m });
            AccountNumber.Add(new AccountNumber() { AccountNumberId = 1, Name = "Poor Man", Number = 123000, DateOfBirth = DateTime.Today.AddDays(-13000), Balance = 0.00m });
        }

        public static List<AccountNumber> AccountNumber {get;set;}

        public static List<AccountNumber> RestrictedAccountNumbers(string[] role, string userName = null)
        {
            if (role.Contains("Admin"))
            {
                if (userName == "superuser@xyz.com")
                {
                    var billsAcoount = AccountNumber.FirstOrDefault(x => x.Name == "Bill Gates");
                    var list = AccountNumber.Where(x=>x.Name != "Bill Gates").ToList();
                    return list;
                }
                else
                {
                    return AccountNumber;
                }

            }
            else return AccountNumber;
        }

    }
}