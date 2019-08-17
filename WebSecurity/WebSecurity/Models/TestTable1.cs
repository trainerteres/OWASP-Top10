using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSecurity.Models
{
    public class TestTable1
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Field1 { get; set; }

        [AllowHtml]
        public string Field2 { get; set; }
    }
}