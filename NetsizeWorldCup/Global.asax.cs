using NetsizeWorldCup.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace NetsizeWorldCup
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        public static void SendMail(string message)
        {
            SmtpService service = new SmtpService();
            service.SendAsync(message);
        }

        protected void Application_Error()
        {
            try
            {
                Exception ex = Server.GetLastError();
                string header = "No Headers";

                try
                {
                    header = "IP=" + HttpContext.Current.Request.UserHostAddress + " User=" + User.Identity.Name + " Error=" + Server.MachineName;
                }
                catch
                {

                }

                SendMail(header + " " + ex.ToString());
            }
            catch
            {

            }
            finally
            {
                Server.ClearError();
#if !DEBUG
                Response.Redirect("/Home/Error");
#endif
            }
        }
    }
}
