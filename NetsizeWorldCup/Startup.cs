using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NetsizeWorldCup.Startup))]
namespace NetsizeWorldCup
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            //app.MapSignalR();
        }

    }
}
