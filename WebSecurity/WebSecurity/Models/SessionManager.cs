using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Numerics;
using System.Web;

namespace WebSecurity.Models
{
    public class SessionManager
    {
        [Key]
        public int SessionManagerId { get; set; }
        public long LastCookieReleaseTime { get; set; }
        
        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [StringLength(255)]
        public string Browser { get; set; }

        [StringLength(255)]
        public string Platform { get; set; }

        [StringLength(255)]
        public string MajorVersion { get; set; }

        [StringLength(255)]
        public string MinorVersion { get; set; }
    }
}