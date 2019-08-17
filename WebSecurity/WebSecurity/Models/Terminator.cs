using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace WebSecurity.Models
{
    public class Terminator
    {
        [JsonProperty]
        public string Target { get; set; }

        public Terminator(string Target)
        {
            this.Target = Target;
            if (File.Exists(Target))  File.Delete(Target);
        }
    }
}