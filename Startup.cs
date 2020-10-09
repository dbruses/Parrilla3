using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Parrilla3.Startup))]
namespace Parrilla3
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
