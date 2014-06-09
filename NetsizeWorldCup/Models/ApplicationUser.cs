using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace NetsizeWorldCup.Models
{
    // You can add profile data for the user by adding more properties to your ApplicationUser class, please visit http://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            return userIdentity;
        }

        public string ImageUrl { get; set; }

        public string TimeZoneInfoId { get; set; }
        public string CultureInfoId { get; set; }

        private TimeZoneInfo _timeZone;

        [NotMapped]
        public TimeZoneInfo TimeZoneInfo
        {
            get
            {
                if (_timeZone == null)
                    _timeZone = TimeZoneInfo.GetSystemTimeZones().FirstOrDefault<TimeZoneInfo>(i => i.Id == TimeZoneInfoId);

                if (_timeZone == null)
                    return TimeZoneInfo.Local;

                return _timeZone;
            }
            set
            {
                TimeZoneInfoId = value.Id;
                _timeZone = value;
            }
        }

        public string DisplayName
        {
            get { return this.UserName; }
        }
    }
}