using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebSecurity.Startup))]
namespace WebSecurity
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
