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

            //// Initializes and seeds the database.
            Database.SetInitializer<ApplicationDbContext>(new ApplicationDbInitializer());

            // Forces initialization of database on model changes.
            using (var context = new ApplicationDbContext())
            {
                context.Database.Initialize(force: true);
            }
        }
    }
}
