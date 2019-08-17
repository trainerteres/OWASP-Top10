using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace WebSecurity.Models
{
    public class EncryptionModel
    {
        public string PlainText { get; set; }

        [AllowHtml]
        public string PrivateKey { get; set; }

        [AllowHtml]
        public string PublicKey { get; set; }

        public string EncryptedText { get; set; }
        public string InitializationVector { get; set; }

    }
}