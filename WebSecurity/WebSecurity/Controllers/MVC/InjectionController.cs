using System;
using System.Configuration;
using System.Data.SqlClient;
using System.Web.Mvc;
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


        [HttpPost]
        public ActionResult AdoNetInjection(FormCollection form)
        {
            var value = form["adoNetNameTxt"].ToString();
            string connectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ToString();
            string commandText = "Insert into TestTable1(Name) Values('" + value + "')";
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