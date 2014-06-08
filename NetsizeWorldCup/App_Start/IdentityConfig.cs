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
                RequireUppercase = true,
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
            return Task.FromResult(0);
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

    public class ApplicationDbInitializer : DropCreateDatabaseIfModelChanges<ApplicationDbContext>
    {
        protected override void Seed(ApplicationDbContext context)
        {
            InitializeIdentityForEF(context);
            InitializeGameForEF(context);

            base.Seed(context);
        }

        protected static Game CreateGame(ApplicationDbContext context, Phase phase, string location, string startDate, string localName, string VisitorName)
        {
            Team localTeam = context.Teams.First<Team>(t => t.Name == localName);
            Team visitorTeam = context.Teams.First<Team>(t => t.Name == VisitorName);

            return new Game { Phase = phase, Location = location, StartDate = DateTime.Parse(startDate), Local = localTeam, Visitor = visitorTeam };
        }

        public static void InitializeGameForEF(ApplicationDbContext context)
        {
            var groups = new List<Group> {
                new Group { Name = "A", ID = 1 },
                new Group { Name = "B", ID = 2 },
                new Group { Name = "C", ID = 3 },
                new Group { Name = "D", ID = 4 },
                new Group { Name = "E", ID = 5 },
                new Group { Name = "F", ID = 6 },
                new Group { Name = "G", ID = 7 },
                new Group { Name = "H", ID = 8 }
            };

            groups.ForEach(g => context.Groups.Add(g));
            context.SaveChanges();

            Group groupA = context.Groups.First<Group>(t => t.Name == "A");
            Group groupB = context.Groups.First<Group>(t => t.Name == "B");
            Group groupC = context.Groups.First<Group>(t => t.Name == "C");
            Group groupD = context.Groups.First<Group>(t => t.Name == "D");
            Group groupE = context.Groups.First<Group>(t => t.Name == "E");
            Group groupF = context.Groups.First<Group>(t => t.Name == "F");
            Group groupG = context.Groups.First<Group>(t => t.Name == "G");
            Group groupH = context.Groups.First<Group>(t => t.Name == "H");


            var phases = new List<Phase> {
                new Phase { ID = 1, Name = "Group"},
                new Phase { ID = 2, Name = "Round of 16"},
                new Phase { ID = 3, Name = "Quarter-final"},
                new Phase { ID = 4, Name = "Semi-final"},
                new Phase { ID = 5, Name = "Final"}
            };

            phases.ForEach(g => context.Phases.Add(g));
            context.SaveChanges();

            Phase phaseGroup = context.Phases.First<Phase>(p => p.Name == "Group");
            Phase phaseEight = context.Phases.First<Phase>(p => p.Name == "Round of 16");
            Phase phaseQuarter = context.Phases.First<Phase>(p => p.Name == "Quarter-final");
            Phase phaseSemi = context.Phases.First<Phase>(p => p.Name == "Semi-final");
            Phase phaseFinal = context.Phases.First<Phase>(p => p.Name == "Final");


            var teams = new List<Team> {
                new Team { Name = "Brazil", Group = groupA, FlagUrl = "http://img.fifa.com/images/flags/4/bra.png" },
                new Team { Name = "Croatia", Group = groupA, FlagUrl = "http://img.fifa.com/images/flags/4/cro.png" },
                new Team { Name = "Mexico", Group = groupA, FlagUrl = "http://img.fifa.com/images/flags/4/mex.png" },
                new Team { Name = "Cameroon", Group = groupA, FlagUrl = "http://img.fifa.com/images/flags/4/cmr.png" },

                new Team { Name = "Spain", Group = groupB, FlagUrl = "http://img.fifa.com/images/flags/4/esp.png" },
                new Team { Name = "Netherlands", Group = groupB, FlagUrl = "http://img.fifa.com/images/flags/4/ned.png" },
                new Team { Name = "Chile", Group = groupB, FlagUrl = "http://img.fifa.com/images/flags/4/chi.png" },
                new Team { Name = "Australia", Group = groupB, FlagUrl = "http://img.fifa.com/images/flags/4/aus.png" },

                new Team { Name = "Colombia", Group = groupC, FlagUrl = "http://img.fifa.com/images/flags/4/col.png" },
                new Team { Name = "Greece", Group = groupC, FlagUrl = "http://img.fifa.com/images/flags/4/gre.png" },
                new Team { Name = "Ivory Coast", Group = groupC, FlagUrl = "http://img.fifa.com/images/flags/4/civ.png" },
                new Team { Name = "Japan", Group = groupC, FlagUrl = "http://img.fifa.com/images/flags/4/jpn.png" },

                new Team { Name = "Uruguay", Group = groupD, FlagUrl = "http://img.fifa.com/images/flags/4/uru.png" },
                new Team { Name = "Costa Rica", Group = groupD, FlagUrl = "http://img.fifa.com/images/flags/4/crc.png" },
                new Team { Name = "England", Group = groupD, FlagUrl = "http://img.fifa.com/images/flags/4/eng.png" },
                new Team { Name = "Italy", Group = groupD, FlagUrl = "http://img.fifa.com/images/flags/4/ita.png" },

                new Team { Name = "Switzerland", Group = groupE, FlagUrl = "http://img.fifa.com/images/flags/4/sui.png" },
                new Team { Name = "Ecuador", Group = groupE, FlagUrl = "http://img.fifa.com/images/flags/4/ecu.png" },
                new Team { Name = "France", Group = groupE, FlagUrl = "http://img.fifa.com/images/flags/4/fra.png" },
                new Team { Name = "Honduras", Group = groupE, FlagUrl = "http://img.fifa.com/images/flags/4/hon.png" },

                new Team { Name = "Argentina", Group = groupF, FlagUrl = "http://img.fifa.com/images/flags/4/arg.png" },
                new Team { Name = "Bosnia-Herzegovina", Group = groupF, FlagUrl = "http://img.fifa.com/images/flags/4/bih.png" },
                new Team { Name = "Iran", Group = groupF, FlagUrl = "http://img.fifa.com/images/flags/4/irn.png" },
                new Team { Name = "Nigeria", Group = groupF, FlagUrl = "http://img.fifa.com/images/flags/4/nga.png" },

                new Team { Name = "Germany", Group = groupG, FlagUrl = "http://img.fifa.com/images/flags/4/ger.png" },
                new Team { Name = "Portugal", Group = groupG, FlagUrl = "http://img.fifa.com/images/flags/4/por.png" },
                new Team { Name = "Ghana", Group = groupG, FlagUrl = "http://img.fifa.com/images/flags/4/gha.png" },
                new Team { Name = "United States", Group = groupG, FlagUrl = "http://img.fifa.com/images/flags/4/United States.png" },

                new Team { Name = "Belgium", Group = groupH, FlagUrl = "http://img.fifa.com/images/flags/4/bel.png" },
                new Team { Name = "Algeria", Group = groupH, FlagUrl = "http://img.fifa.com/images/flags/4/alg.png" },
                new Team { Name = "Russia", Group = groupH, FlagUrl = "http://img.fifa.com/images/flags/4/rus.png" },
                new Team { Name = "South Korea", Group = groupH, FlagUrl = "http://img.fifa.com/images/flags/4/kor.png" }};

            teams.ForEach(g => context.Teams.Add(g));
            context.SaveChanges();

            var games = new List<Game> {
                    CreateGame(context, phaseGroup, "Arena Corinthians","2014-06-12 20:00:00","Brazil","Croatia"),
                    CreateGame(context, phaseGroup, "Estadio Mineirao","2014-06-17 16:00:00","Belgium","Algeria"),
                    CreateGame(context, phaseGroup, "Estadio Beira-Rio","2014-06-22 19:00:00","South Korea","Algeria"),
                    CreateGame(context, phaseGroup, "Arena Pantanal","2014-06-13 22:00:00","Chile","Australia"),
                    CreateGame(context, phaseGroup, "Estadio Beira-Rio","2014-06-25 16:00:00","Nigeria","Argentina"),
                    CreateGame(context, phaseGroup, "Estadio das Dunas","2014-06-13 16:00:00","Mexico","Cameroon"),
                    CreateGame(context, phaseGroup, "Estadio Mineirao","2014-06-14 13:00:00","Colombia","Greece"),
                    CreateGame(context, phaseGroup, "Estadio Nacional","2014-06-15 16:00:00","Switzerland","Ecuador"),
                    CreateGame(context, phaseGroup, "Arena Fonte Nova","2014-06-16 16:00:00","Germany","Portugal"),
                    CreateGame(context, phaseGroup, "Estadio Beira-Rio","2014-06-18 16:00:00","Australia","Netherlands"),
                    CreateGame(context, phaseGroup, "Estadio Nacional","2014-06-19 16:00:00","Colombia","Ivory Coast"),
                    CreateGame(context, phaseGroup, "Arena Pernambuco","2014-06-20 16:00:00","Italy","Costa Rica"),
                    CreateGame(context, phaseGroup, "Estadio Mineirao","2014-06-21 16:00:00","Argentina","Iran"),
                    CreateGame(context, phaseGroup, "Estádio Jornalista Mário Filho","2014-06-22 16:00:00","Belgium","Russia"),
                    CreateGame(context, phaseGroup, "Arena Corinthians","2014-06-23 16:00:00","Netherlands","Chile"),
                    CreateGame(context, phaseGroup, "Arena da Baixada","2014-06-23 16:00:00","Australia","Spain"),
                    CreateGame(context, phaseGroup, "Estadio Mineirao","2014-06-24 16:00:00","Costa Rica","England"),
                    CreateGame(context, phaseGroup, "Estadio das Dunas","2014-06-24 16:00:00","Italy","Uruguay"),
                    CreateGame(context, phaseGroup, "Arena Fonte Nova","2014-06-25 16:00:00","Bosnia-Herzegovina","Iran"),
                    CreateGame(context, phaseGroup, "Arena Pernambuco","2014-06-26 16:00:00","United States","Germany"),
                    CreateGame(context, phaseGroup, "Estadio Nacional","2014-06-26 16:00:00","Portugal","Ghana"),
                    CreateGame(context, phaseGroup, "Arena Fonte Nova","2014-06-13 19:00:00","Spain","Netherlands"),
                    CreateGame(context, phaseGroup, "Estadio Castelao","2014-06-14 19:00:00","Uruguay","Costa Rica"),
                    CreateGame(context, phaseGroup, "Estadio Beira-Rio","2014-06-15 19:00:00","France","Honduras"),
                    CreateGame(context, phaseGroup, "Arena de Baixada","2014-06-16 19:00:00","Iran","Nigeria"),
                    CreateGame(context, phaseGroup, "Estadio Castelao","2014-06-17 19:00:00","Brazil","Mexico"),
                    CreateGame(context, phaseGroup, "Estádio Jornalista Mário Filho","2014-06-18 19:00:00","Spain","Chile"),
                    CreateGame(context, phaseGroup, "Arena Corinthians","2014-06-19 19:00:00","Uruguay","England"),
                    CreateGame(context, phaseGroup, "Arena Fonte Nova","2014-06-20 19:00:00","Switzerland","France"),
                    CreateGame(context, phaseGroup, "Estadio Castelao","2014-06-21 19:00:00","Germany","Ghana"),
                    CreateGame(context, phaseGroup, "Arena Pantanal","2014-06-24 20:00:00","Japan","Colombia"),
                    CreateGame(context, phaseGroup, "Arena Amazonia","2014-06-25 20:00:00","Honduras","Switzerland"),
                    CreateGame(context, phaseGroup, "Arena Corinthians","2014-06-26 20:00:00","South Korea","Belgium"),
                    CreateGame(context, phaseGroup, "Estadio Nacional","2014-06-23 20:00:00","Cameroon","Brazil"),
                    CreateGame(context, phaseGroup, "Arena Pernambuco","2014-06-23 20:00:00","Croatia","Mexico"),
                    CreateGame(context, phaseGroup, "Estadio Castelao","2014-06-24 20:00:00","Greece","Ivory Coast"),
                    CreateGame(context, phaseGroup, "Estádio Jornalista Mário Filho","2014-06-25 20:00:00","Ecuador","France"),
                    CreateGame(context, phaseGroup, "Arena da Baixada","2014-06-26 20:00:00","Algeria","Russia"),
                    CreateGame(context, phaseGroup, "Arena Amazonia","2014-06-14 22:00:00","England","Italy"),
                    CreateGame(context, phaseGroup, "Arena Pantanal","2014-06-17 22:00:00","Russia","South Korea"),
                    CreateGame(context, phaseGroup, "Arena Amazonia","2014-06-18 22:00:00","Cameroon","Croatia"),
                    CreateGame(context, phaseGroup, "Arena Pantanal","2014-06-21 22:00:00","Nigeria","Bosnia-Herzegovina"),
                    CreateGame(context, phaseGroup, "Arena Amazonia","2014-06-22 22:00:00","United States","Portugal"),
                    CreateGame(context, phaseGroup, "Estádio Jornalista Mário Filho","2014-06-15 22:00:00","Argentina","Bosnia-Herzegovina"),
                    CreateGame(context, phaseGroup, "Estadio das Dunas","2014-06-16 22:00:00","Ghana","United States"),
                    CreateGame(context, phaseGroup, "Estadio das Dunas","2014-06-19 22:00:00","Japan","Greece"),
                    CreateGame(context, phaseGroup, "Arena da Baixada","2014-06-20 22:00:00","Honduras","Ecuador"),
                    CreateGame(context, phaseGroup, "Arena Pernambuco","2014-06-15 01:00:00","Ivory Coast","Japan")
                };

            games.ForEach(g => context.Games.Add(g));
            context.SaveChanges();
        }

        public static void InitializeIdentityForEF(ApplicationDbContext db)
        {
            var userManager = HttpContext
                .Current.GetOwinContext()
                .GetUserManager<ApplicationUserManager>();

            var roleManager = HttpContext.Current
                .GetOwinContext()
                .Get<ApplicationRoleManager>();

            const string name = "admin@admin.com";
            const string password = "Admin@123456";
            const string roleName = "Admin";

            //    //Create Role Admin if it does not exist
            //    var role = roleManager.FindByName(roleName);

            //    if (role == null)
            //    {
            //        role = new IdentityRole(roleName);
            //        var roleresult = roleManager.Create(role);
            //    }

            var user = userManager.FindByName(name);

            if (user == null)
            {
                user = new ApplicationUser { UserName = name, Email = name, TimeZoneInfo = TimeZoneInfo.Local };
                var result = userManager.Create(user, password);
                result = userManager.SetLockoutEnabled(user.Id, false);
            }

            //    // Add user admin to Role Admin if not already added
            //    var rolesForUser = userManager.GetRoles(user.Id);

            //    if (!rolesForUser.Contains(role.Name))
            //    {
            //        var result = userManager.AddToRole(user.Id, role.Name);
            //    }
        }
    }
}
