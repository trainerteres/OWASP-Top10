using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Claims;
using System.Security.Principal;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Mvc;
using System.Web.Mvc.Filters;
using WebSecurity.Models;
using WebSecurity.ReverseEngineering;
using System.Numerics;

namespace WebSecurity.Filter
{

    public class CustomAuthAttribute : IAuthenticationFilter
    {
        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.Current.GetOwinContext().Authentication;
            }
        }

        public void OnAuthentication(AuthenticationContext filterContext)
        {
            if (filterContext.HttpContext.User.Identity.IsAuthenticated)
            {
                // Get Existing Cookie
                var cookie = filterContext.HttpContext.Request.Cookies[CookieExtractor.CookieName];

                var identity = new ClaimsIdentity(filterContext.HttpContext.User.Identity);
                //var claims = CookieExtractor.GetClaimsFromTicket(cookie.Value);

                var lastIssuedOnClaim = identity.Claims.FirstOrDefault(x => x.Type == CookieExtractor.Claim_LastIssuedOn);

                var currentDate = DateTime.Now;
                var validUser = true;
                // Check the browser info and last issued on parameters
                // If Valid then, Update SessionManager table and replace cookie

                ApplicationUser currentUser = new ApplicationUser();

                using (var appDb = new ApplicationDbContext())
                {
                    var UserManager = new ApplicationUserManager(new UserStore<ApplicationUser>(appDb));
                    currentUser = UserManager.FindByNameAsync(filterContext.HttpContext.User.Identity.Name).Result;
                    
                    using (var db = new TestDbContext())
                    {
                        var existingSession = db.SessionManager.AsNoTracking().FirstOrDefault(x => x.UserId == currentUser.Id);

                        validUser = existingSession.Browser == filterContext.HttpContext.Request.Browser.Browser
                                    && existingSession.Platform == filterContext.HttpContext.Request.Browser.Platform
                                    && existingSession.MajorVersion == filterContext.HttpContext.Request.Browser.MajorVersion.ToString()
                                    && existingSession.MinorVersion == filterContext.HttpContext.Request.Browser.MinorVersion.ToString()
                                    && existingSession.LastCookieReleaseTime == Convert.ToInt64(lastIssuedOnClaim.Value);

                        if (!validUser)
                        {
                            filterContext.HttpContext.Session.RemoveAll();
                            filterContext.HttpContext.Session.Clear();
                            filterContext.HttpContext.Session.Abandon();

                            filterContext.HttpContext.User = new GenericPrincipal(new GenericIdentity(string.Empty), null);

                            filterContext.HttpContext.Response.Cookies.Remove(CookieExtractor.CookieName);

                            AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);

                            filterContext.Result = new RedirectResult("~/Error");
                        }
                        else
                        {

                            var sessionMgr = new SessionManager();
                            sessionMgr.UserId = currentUser.Id;
                            sessionMgr.LastCookieReleaseTime = currentDate.Ticks;
                            sessionMgr.Browser = filterContext.HttpContext.Request.Browser.Browser.ToString();
                            sessionMgr.Platform = filterContext.HttpContext.Request.Browser.Platform;
                            sessionMgr.MajorVersion = filterContext.HttpContext.Request.Browser.MajorVersion.ToString();
                            sessionMgr.MinorVersion = filterContext.HttpContext.Request.Browser.MinorVersion.ToString();

                            if (existingSession != null)
                            {
                                sessionMgr.SessionManagerId = existingSession.SessionManagerId;

                                db.Entry(existingSession).State = System.Data.Entity.EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                db.SessionManager.Add(sessionMgr);
                                db.SaveChanges();
                            }
                        }
                    }

                    if (validUser)
                    {
                        var persistantSession = filterContext.HttpContext.Session[CookieExtractor.IsPersistent];

                        var authProperties = new AuthenticationProperties();

                        if (persistantSession != null)
                        {
                            authProperties.IsPersistent = Convert.ToBoolean(persistantSession.ToString());
                        }

                        identity = new ClaimsIdentity(filterContext.HttpContext.User.Identity);

                        identity.AddClaim(new Claim(CookieExtractor.Claim_LastIssuedOn, currentDate.ToString()));

                        var authTicket = new AuthenticationTicket(identity, authProperties);

                        authTicket.Identity.AddClaim(new Claim(CookieExtractor.Claim_LastIssuedOn, currentDate.ToString()));

                        var newCookieValue = CookieExtractor.CreateCookieValueFromTicket(authTicket);

                        HttpCookie newCookie = new HttpCookie(CookieExtractor.CookieName, newCookieValue);

                        filterContext.HttpContext.Response.Cookies.Remove(CookieExtractor.CookieName);
                        filterContext.HttpContext.Response.Cookies.Add(newCookie);
                    }
                }
            }
        }

        public void OnAuthenticationChallenge(AuthenticationChallengeContext filterContext)
        {
            var user = filterContext.HttpContext.User;

            if(user == null && !user.Identity.IsAuthenticated)
            {
                filterContext.Result = new RedirectResult("~/Login");
            }
        }
    }
}