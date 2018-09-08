using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ventascatalogo.Backend.Startup))]
namespace ventascatalogo.Backend
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
