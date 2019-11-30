using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Gazzetta.Startup))]
namespace Gazzetta
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
