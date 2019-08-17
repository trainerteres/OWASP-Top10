using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;

namespace WebSecurity.Controllers.MVC
{
    public class InsecureDeserializationController : Controller
    {
        // GET: InsecureDeserialization
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        // {"$type": "WebSecurity.Models.Terminator, WebSecurity","Target":"C:\\Test\\InsecureDeserialization.txt"}
        public ActionResult InsecureRequest(string value)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings
            {
                PreserveReferencesHandling = PreserveReferencesHandling.Objects,
                Formatting = Formatting.Indented,
                TypeNameHandling = TypeNameHandling.All
            };

            var postedObject = JsonConvert.DeserializeObject(value, settings);

            return View("Index");
        }
    }
}