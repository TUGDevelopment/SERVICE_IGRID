using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PLL.Startup))]
namespace PLL
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
