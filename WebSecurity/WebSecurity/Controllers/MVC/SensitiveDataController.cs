using System;
using System.Security.Cryptography;
using System.Web;
using System.Web.Mvc;
using WebSecurity.Encryption;
using WebSecurity.Models;

namespace WebSecurity.Controllers
{
    public class SensitiveDataController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult ServeAjaxRequest(string data)
        {
            return Json($"The input was given as {data}", JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SymmetricEncrypt(WebSecurity.Models.EncryptionModel encryption, string ButtonType)
        {
            var result = new EncryptionModel();

            var symEncryption = new SymmetricEncryption();

            SymmetricAlgorithm symAlgo = new AesManaged();
            symAlgo.Padding = PaddingMode.PKCS7;
            var key = string.Empty;
            var iv = string.Empty;

            if (!string.IsNullOrEmpty(encryption.PrivateKey) && !string.IsNullOrEmpty(encryption.InitializationVector))
            {
                symAlgo.Key = Convert.FromBase64String(encryption.PrivateKey);
                symAlgo.IV = Convert.FromBase64String(encryption.InitializationVector);
            }
            else
            {
                key = Convert.ToBase64String(symAlgo.Key);
                iv = Convert.ToBase64String(symAlgo.IV);
            }

            if (ButtonType == "Encrypt")
            {
                var encryptedText = symEncryption.EncryptText(symAlgo, encryption.PlainText);
                result = new EncryptionModel()
                {
                    EncryptedText = encryptedText,
                    PlainText = encryption.PlainText,
                    InitializationVector = iv,
                    PrivateKey = key
                };
            }
            else if (ButtonType == "Decrypt")
            {
                var plainText = symEncryption.DecryptData(symAlgo, encryption.EncryptedText);

                result = new EncryptionModel()
                {
                    EncryptedText = encryption.EncryptedText,
                    PlainText = plainText,
                    PrivateKey = encryption.PrivateKey,
                    InitializationVector = encryption.InitializationVector
                };
            }

            return Json(result);
        }

        [HttpPost]
        public JsonResult AsymmetricEncrypt(WebSecurity.Models.EncryptionModel encryption, string ButtonType)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();

            string publicKey = string.Empty; // false to get the public key   
            string privateKey = string.Empty; // true to get the private key   

            if (!string.IsNullOrEmpty(encryption.PublicKey) && !string.IsNullOrEmpty(encryption.PrivateKey))
            {
                publicKey = HttpUtility.HtmlDecode(encryption.PublicKey);
                privateKey = HttpUtility.HtmlDecode(encryption.PrivateKey);
            }
            else
            {
                publicKey = rsa.ToXmlString(false); // false to get the public key   
                privateKey = rsa.ToXmlString(true); // true to get the private key   
            }

            var result = new EncryptionModel();

            var asymEncryption = new AsymmetricEncryption();

            if (ButtonType == "Encrypt")
            {
                var encryptedText = asymEncryption.EncryptText(publicKey, encryption.PlainText);
                result = new EncryptionModel()
                {
                    EncryptedText = encryptedText,
                    PlainText = encryption.PlainText,
                    PublicKey = HttpUtility.HtmlEncode(publicKey),
                    PrivateKey = HttpUtility.HtmlEncode(privateKey)
                };
            }
            else if (ButtonType == "Decrypt")
            {
                var plainText = asymEncryption.DecryptData(HttpUtility.HtmlDecode(encryption.PrivateKey), encryption.EncryptedText);

                result = new EncryptionModel()
                {
                    EncryptedText = encryption.EncryptedText,
                    PlainText = plainText,
                    PublicKey = HttpUtility.HtmlEncode(publicKey),
                    PrivateKey = HttpUtility.HtmlEncode(privateKey)
                };
            }

            return Json(result);
        }
    }
}