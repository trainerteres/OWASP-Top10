using Microsoft.Owin.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Owin;

namespace WebSecurity.Auth
{
    public class CustomCookieManager : ICookieManager
    {
        public CustomCookieManager()
        {
            
        }

        public void AppendResponseCookie(IOwinContext context, string key, string value, CookieOptions options)
        {
            context.Response.Cookies.Append(key, value, options);
        }

        public void DeleteCookie(IOwinContext context, string key, CookieOptions options)
        {
            
        }

        public string GetRequestCookie(IOwinContext context, string key)
        {
            return context.Request.Cookies[key];
        }
    }
}