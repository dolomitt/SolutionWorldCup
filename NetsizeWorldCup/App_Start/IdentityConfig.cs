using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using NetsizeWorldCup.Models;
using System.Data.Entity;
using System.Web;
using System.Collections.Generic;
using System.Linq;
using System;
using System.Net;
using System.Net.Mail;

namespace NetsizeWorldCup
{
    // Configure the application user manager used in this application. UserManager is defined in ASP.NET Identity and is used by the application.

    public class ApplicationUserManager : UserManager<ApplicationUser>
    {
        public ApplicationUserManager(IUserStore<ApplicationUser> store)
            : base(store)
        {
        }

        public static ApplicationUserManager Create(IdentityFactoryOptions<ApplicationUserManager> options, IOwinContext context)
        {
            var manager = new ApplicationUserManager(new UserStore<ApplicationUser>(context.Get<ApplicationDbContext>()));
            // Configure validation logic for usernames
            manager.UserValidator = new UserValidator<ApplicationUser>(manager)
            {
                AllowOnlyAlphanumericUserNames = false,
                RequireUniqueEmail = true
            };
            // Configure validation logic for passwords
            manager.PasswordValidator = new PasswordValidator
            {
                RequiredLength = 6,
                RequireNonLetterOrDigit = true,
                RequireDigit = true,
                RequireLowercase = true,
                RequireUppercase = false,
            };
            // Register two factor authentication providers. This application uses Phone and Emails as a step of receiving a code for verifying the user
            // You can write your own provider and plug in here.
            manager.RegisterTwoFactorProvider("PhoneCode", new PhoneNumberTokenProvider<ApplicationUser>
            {
                MessageFormat = "Your security code is: {0}"
            });
            manager.RegisterTwoFactorProvider("EmailCode", new EmailTokenProvider<ApplicationUser>
            {
                Subject = "Security Code",
                BodyFormat = "Your security code is: {0}"
            });
            manager.EmailService = new EmailService();
            manager.SmsService = new SmsService();
            var dataProtectionProvider = options.DataProtectionProvider;
            if (dataProtectionProvider != null)
            {
                manager.UserTokenProvider = new DataProtectorTokenProvider<ApplicationUser>(dataProtectionProvider.Create("ASP.NET Identity"));
            }

            return manager;
        }
    }

    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your email service here to send an email.
            return configSendGridasync(message);
        }

        private async Task<bool> configSendGridasync(IdentityMessage message)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add(message.Destination);
            mailMsg.From = new System.Net.Mail.MailAddress("admin@gto16.com", "Admin");
            mailMsg.Bcc.Add("dolomitt@gmail.com");
            mailMsg.Subject = message.Subject;
            mailMsg.Body = message.Body;
            mailMsg.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("localhost", 25);
            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("dolomitt@gmail.com", "TAZ@718#call");
            //smtpClient.Credentials = credentials;
            //smtpClient.EnableSsl = true;

            smtpClient.Send(mailMsg);

            await Task.Yield();
            return true;
        }    
    }

    public class SmtpService
    {
        public Task SendAsync(string message)
        {
            // Plug in your email service here to send an email.
            return sendAsync(message);
        }

        private async Task<bool> sendAsync(string message)
        {
            MailMessage mailMsg = new MailMessage();
            mailMsg.To.Add("dolomitt@gmail.com");
            mailMsg.From = new System.Net.Mail.MailAddress("admin@gto16.com", "Admin");
            mailMsg.Subject = "NS Euro Cup 2016 - Error";
            mailMsg.Body = message;
            mailMsg.IsBodyHtml = true;

            SmtpClient smtpClient = new SmtpClient("localhost", 25);
            //System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("dolomitt@gmail.com", "TAZ@718#call");
            //smtpClient.Credentials = credentials;
            //smtpClient.EnableSsl = true;

            smtpClient.Send(mailMsg);

            await Task.Yield();
            return true;
        }
    }

    public class SmsService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            // Plug in your sms service here to send a text message.
            return Task.FromResult(0);
        }
    }

    public class ApplicationRoleManager : RoleManager<IdentityRole>
    {
        public ApplicationRoleManager(IRoleStore<IdentityRole, string> roleStore)
            : base(roleStore)
        {

        }

        public static ApplicationRoleManager Create(IdentityFactoryOptions<ApplicationRoleManager> options, IOwinContext context)
        {
            var manager = new ApplicationRoleManager(new RoleStore<IdentityRole>(context.Get<ApplicationDbContext>()));
            return manager;
        }

    }
}
