using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Web.Http;
using System.Web.Http.Description;
using WebSecurity.Models;

namespace WebSecurity.Controllers.API
{
    public class TestTable1Controller : ApiController
    {
        private TestDbContext db = new TestDbContext();

        // GET: api/TestTable1
        [Authorize]
        public IQueryable<TestTable1> GetTestTable1()
        {
            return db.TestTable1;
        }

        // GET: api/TestTable1/5
        [ResponseType(typeof(TestTable1))]
        public IHttpActionResult GetTestTable1(int id)
        {
            TestTable1 testTable1 = db.TestTable1.Find(id);
            if (testTable1 == null)
            {
                return NotFound();
            }

            return Ok(testTable1);
        }

        // PUT: api/TestTable1/5
        [ResponseType(typeof(void))]
        public IHttpActionResult PutTestTable1(int id, TestTable1 testTable1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != testTable1.Id)
            {
                return BadRequest();
            }

            db.Entry(testTable1).State = EntityState.Modified;

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestTable1Exists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return StatusCode(HttpStatusCode.NoContent);
        }

        // POST: api/TestTable1
        [ResponseType(typeof(TestTable1))]
        public IHttpActionResult PostTestTable1(TestTable1 testTable1)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            db.TestTable1.Add(testTable1);
            db.SaveChanges();

            return CreatedAtRoute("DefaultApi", new { id = testTable1.Id }, testTable1);
        }

        // DELETE: api/TestTable1/5
        [ResponseType(typeof(TestTable1))]
        public IHttpActionResult DeleteTestTable1(int id)
        {
            TestTable1 testTable1 = db.TestTable1.Find(id);
            if (testTable1 == null)
            {
                return NotFound();
            }

            db.TestTable1.Remove(testTable1);
            db.SaveChanges();

            return Ok(testTable1);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        private bool TestTable1Exists(int id)
        {
            return db.TestTable1.Count(e => e.Id == id) > 0;
        }
    }
}