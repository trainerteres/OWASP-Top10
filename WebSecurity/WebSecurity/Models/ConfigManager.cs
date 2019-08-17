using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace WebSecurity.Models
{
    public class ConfigManager
    {
        [Key]
        public int ConfigId { get; set; }

        [StringLength(1000)]
        public string Key { get; set; }

        [StringLength(1000)]
        public string Value { get; set; }
    }
}