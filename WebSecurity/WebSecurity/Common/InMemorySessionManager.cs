using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebSecurity.Models;

namespace WebSecurity.Common
{
    public class InMemorySessionManager
    {
        private static List<SessionManager> SessionManager { get; set; }

        /// <summary>
        /// Update session data
        /// </summary>
        /// <param name="sessionManager">Session Manager</param>
        public void UpdateSession(SessionManager sessionManager)
        {

        }
    }
}