using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BotClientWeb.Startup))]
namespace BotClientWeb
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
