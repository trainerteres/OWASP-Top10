using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.Mvc;
using Microsoft.Security.Application;
using WebSecurity.Models;

namespace WebSecurity.Controllers
{
    public class CrossSiteScriptController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        // <image width="300" height="300" src="https://images.unsplash.com/photo-1502367448277-82e29b176948?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=500&q=60"/>
        public ActionResult ReflectedJsonForm(FormCollection form)
        {
            var value = form["reflectedNameTxt"].ToString();
            return Json(value);
        }


        [HttpPost]
        // <image width="300" height="300" src="https://images.unsplash.com/photo-1502367448277-82e29b176948?ixlib=rb-1.2.1&ixid=eyJhcHBfaWQiOjEyMDd9&auto=format&fit=crop&w=500&q=60"/>
        public JsonResult ReflectedJson(string name)
        {
            return Json(name);
        }

        [HttpPost]
        public JsonResult PersistedJson(string name)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            string commandText = "Insert into TestTable1(Name, Field2) Values('Test1','" + name + "')";
            // test'); DELETE FROM TestTable1; PRINT('Hacked!'

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                using (SqlCommand comm = new SqlCommand(commandText, conn))
                {
                    comm.ExecuteNonQuery();
                }
                conn.Close();
            }

            using (var db = new TestDbContext())
            {
                var nameInDb = db.TestTable1.FirstOrDefault(x => x.Name == "Test1");
                if (nameInDb!=null)
                {
                    return Json(nameInDb.Field2);
                }
            }

            return Json(string.Empty);
        }

        [HttpGet]
        public ActionResult SecureXSS()
        {
            return View(new TestTable1());
        }

        [HttpPost]
        public ActionResult SecureXSS(TestTable1 testTable)
        {
            var testData = new TestTable1();
            using (var db = new TestDbContext())
            {
                testTable.Field2 = Encoder.HtmlEncode(testTable.Field2);
                db.TestTable1.Add(testTable);
                db.SaveChanges();

                testData = db.TestTable1.OrderByDescending(x => x.Id).FirstOrDefault(x=>x.Name == testTable.Name);
            }

            testData.Field2 = HttpUtility.HtmlDecode(testData.Field2);
            return View(testData);
        }
    }
}