using Microsoft.AspNet.Identity;
using Microsoft.Owin;
using NetsizeWorldCup.Models;
using Owin;
using System.Web;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin;

[assembly: OwinStartupAttribute(typeof(NetsizeWorldCup.Startup))]
namespace NetsizeWorldCup
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }

    }
}
