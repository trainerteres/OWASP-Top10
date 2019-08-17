using MvcContrib.ActionResults;
using System;
using System.IO;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;
using WebSecurity.Models;

namespace WebSecurity.Controllers
{
    public class XmlExternalEntitiesController : Controller
    {
        // GET: XmlExternalEntities
        public ActionResult Index()
        {
            var student = new StudentXxe();
            return View(student);
        }

        private FileResult GetFile(string fileName)
        {
            var filepath = AppDomain.CurrentDomain.BaseDirectory + $"bin/Content/Files/{fileName}";
            byte[] fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/txt", fileName);
        }

        private StudentXxe DeserializeStudent(string xml)
        {
            var student = new StudentXxe();
            TextReader textReader = new StringReader(xml);
            var settings = new XmlReaderSettings { DtdProcessing = DtdProcessing.Parse };
            var serializer = new XmlSerializer(typeof(StudentXxe));

            using (var xmlReader = XmlReader.Create(textReader, settings))
            {
                student = (StudentXxe)serializer.Deserialize(xmlReader);
            }

            return student;
        }

        [HttpGet]
        public FileResult InnocentXml()
        {
            return GetFile("Innocent.xml");
        }

        [HttpGet]
        public FileResult InternalEntitiesXml()
        {
            return GetFile("InternalEntities.xml");
        }

        [HttpGet]
        public FileResult ExternalEntitiesXml()
        {
            return GetFile("ExternalEntities.xml");
        }

        [HttpGet]
        public ContentResult GetDtd()
        {
            //var fileName = "MaliciousDtd.dtd";
            //var filepath = AppDomain.CurrentDomain.BaseDirectory + $"bin/Content/Files/{fileName}";
            //string text = System.IO.File.ReadAllText(filepath);
            return Content("malicious value may be load of data.");
        }

        [HttpPost]
        public ActionResult UploadFile(HttpPostedFileBase file)
        {
            try
            {
                var student = new StudentXxe();
                if (file.ContentLength > 0)
                {
                    string xmlText = new StreamReader(file.InputStream).ReadToEnd();
                    student = DeserializeStudent(xmlText);
                }
                ViewBag.Message = "File Uploaded Successfully!!";
                return View("Index",student);
            }
            catch(Exception ex)
            {
                ViewBag.Message = "File upload failed!!";
                return View("Index");
            }
        }
    }
}