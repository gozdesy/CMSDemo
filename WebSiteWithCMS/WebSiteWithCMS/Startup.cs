using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebSiteWithCMS.Startup))]
namespace WebSiteWithCMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
