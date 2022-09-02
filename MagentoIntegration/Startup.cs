using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MagentoIntegration.Startup))]
namespace MagentoIntegration
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
