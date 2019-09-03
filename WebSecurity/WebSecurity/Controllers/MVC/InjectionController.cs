using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using System.Xml.Linq;
using System.Xml.XPath;
using WebSecurity.Models;

namespace WebSecurity.Controllers
{
    public class InjectionController : Controller
    {
        // GET: Injection
        public ActionResult Index()
        {
            return View();
        }

        private void SetElementValue(XElement xElement, string value)
        {
            xElement.SetElementValue("state", value);
        }


        [HttpPost]
        public ActionResult AdoNetInjection(FormCollection form)
        {
            var value = form["adoNetNameTxt"].ToString();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            string commandText = "Insert into TestTable1(Name) Values('" + value + "')";
            // INSERT INTO TestTable1 (Name) VALUES ('Test Value');
            // test'); DELETE TestTable1; PRINT('Hacked!

            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    using (SqlCommand comm = new SqlCommand(commandText, conn))
                    {
                        comm.ExecuteNonQuery();
                    }
                    conn.Close();
                }

            }
            catch(Exception ex)
            {
                var ex1 = ex;
            }

            return View("Index");
        }


        // Normal Input: ReportsService, FinancialService
        // Hacker Input: ' or 1=1 or text()='
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult XPathInjection(FormCollection form)
        {
            var filePath = AppDomain.CurrentDomain.BaseDirectory + $"bin/Content/Files/XPathXml.xml";
            var services = form["xpathQuery"].ToString();
            var selectedSwitch = form["xpathSwitch"].ToString();

            var servicesArray = services.Split(',');

            foreach (var service in servicesArray)
            {
                var document = new XPathDocument(filePath);
                var navigator = document.CreateNavigator();

                var expression = $@"/services/service/name[text()='{service}']";

                XPathExpression expr = navigator.Compile(expression);
                XPathNodeIterator iterator = navigator.Select(expr);

                var list = new List<string>();

                var xDoc = XDocument.Load(filePath);

                foreach (XPathNavigator node in iterator)
                {
                    var element = xDoc.Descendants("services").Descendants("service").FirstOrDefault(x => x.Element("name").Value == node.Value);
                    SetElementValue(element, selectedSwitch);
                    xDoc.Save(filePath);
                }
            }

            return View("Index");
        }

        [HttpPost]
        public ActionResult EFInjection(FormCollection form)
        {
            var value = form["efInjectionTxt"].ToString();
            string commandText = "Insert into TestTable1(Name) Values('" + value + "')";

            using (var db = new TestDbContext())
            {
                var selection = db.Database.ExecuteSqlCommand(commandText);
            }

            return View("Index");
        }
    }
}