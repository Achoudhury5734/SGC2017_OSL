using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(OSL.AdminPortal.Startup))]
namespace OSL.AdminPortal
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
