using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Linq;
using System.Security.Claims;
using System.Web;
using WebSecurity.Models;
using WebSecurity.ReverseEngineering;

namespace WebSecurity.Auth
{
    public class CustomTicketDataFormat : ISecureDataFormat<AuthenticationTicket>
    {

        public string Protect(AuthenticationTicket ticket)
        {
            //Ticket value serialized here will be the cookie sent. Encryption stage.
            //Make any changes if you wish to the ticket
            var currentDate = DateTime.Now;
            ApplicationUser currentUser = new ApplicationUser();

            using (var db = new ApplicationDbContext())
            {
                var UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(db));
                currentUser = UserManager.FindByNameAsync(ticket.Identity.Name).Result;
            }


            using (var db = new TestDbContext())
            {
                var sessionMgr = new SessionManager();
                sessionMgr.UserId = currentUser.Id;
                sessionMgr.LastCookieReleaseTime = currentDate.Ticks;
                sessionMgr.Browser = HttpContext.Current.Request.Browser.Browser.ToString();
                sessionMgr.Platform = HttpContext.Current.Request.Browser.Platform;
                sessionMgr.MajorVersion = HttpContext.Current.Request.Browser.MajorVersion.ToString();
                sessionMgr.MinorVersion = HttpContext.Current.Request.Browser.MinorVersion.ToString();
                
                if (db.SessionManager.Any(x=> x.UserId == sessionMgr.UserId))
                {
                    var session = db.SessionManager.AsNoTracking().FirstOrDefault(x => x.UserId == sessionMgr.UserId);
                    sessionMgr.SessionManagerId = session.SessionManagerId;
                    session = sessionMgr;

                    db.Entry(session).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    db.SessionManager.Add(sessionMgr);
                    db.SaveChanges();
                }
            }
            
            var identity = new ClaimsIdentity(ticket.Identity);

            //identity.AddClaim(new Claim(CookieExtractor.Claim_LastIssuedOn, currentDate.ToString()));

            ticket.Identity.AddClaim(new Claim(CookieExtractor.Claim_LastIssuedOn, currentDate.Ticks.ToString()));

            return CookieExtractor.CreateCookieValueFromTicket(ticket);
        }

        public AuthenticationTicket Unprotect(string cookieValue)
        {
            //Invoked everytime when a cookie string is being converted to a AuthenticationTicket. 
            var props = new AuthenticationProperties();

            return CookieExtractor.CreateTicketFromCookie(cookieValue, CookieExtractor.AuthenticationType, props);
        }
    }
}