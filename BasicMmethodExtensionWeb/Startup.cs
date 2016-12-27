using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BasicMmethodExtensionWeb.Startup))]
namespace BasicMmethodExtensionWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
