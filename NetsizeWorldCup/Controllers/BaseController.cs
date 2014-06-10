using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Owin;
using NetsizeWorldCup.Models;
using System.Threading;
using System.Globalization;

namespace NetsizeWorldCup.Controllers
{
    [InternationalizationAttribute]
    public abstract class BaseController : Controller
    {
        protected ApplicationDbContext db { get; set; }
        protected ApplicationUserManager _userManager;

        public ApplicationUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();
            }
            protected set
            {
                _userManager = value;
            }
        }

        public BaseController()
        {
            db = new ApplicationDbContext();
        }
    }
}