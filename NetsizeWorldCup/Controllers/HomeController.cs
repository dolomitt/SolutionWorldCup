using Google.Apis.Analytics.v3;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using Microsoft.AspNet.Identity;
using NetsizeWorldCup.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Syndication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;
using System.Xml;
using System.Xml.Serialization;

namespace NetsizeWorldCup.Controllers
{
    public static class CacheEnum
    {
        public const string UserCount = "UserCount";
        public const string RemainingGameCount = "RemainingGameCount";
        public const string Scores = "Scores";
        public const string WeatherInfo = "WeatherInfo";
        public const string LastFeeds = "LastFeeds";
        public const string BetCount = "BetCount";
        public const string TeamList = "TeamList";
        public const string GooglePageViews = "GooglePageViews";
    }

    public class HomeController : BaseController
    {
        static object _syncRootWeather = new object();
        static object _syncRootFeed = new object();

        public HomeController()
            : base()
        { }

        Dictionary<string, Models.Betclic.Match> games;

        [AllowAnonymous]
        public async Task<ActionResult> Index()
        {
            string currentUserId = User.Identity.GetUserId();
            var user = await db.Users.FirstOrDefaultAsync<ApplicationUser>(u => u.Id == currentUserId);

            if (user != null)
                ViewBag.CurrentTimeZoneInfo = user.TimeZoneInfo;
            else
                ViewBag.CurrentTimeZoneInfo = TimeZoneInfo.Local;

            ViewBag.Feeds = GetLastFeeds();
            ViewBag.WeatherInfo = GetWeatherInfo();
            ViewBag.UserCount = await GetUserCount();
            ViewBag.RemainingGameCount = await GetRemainingGameCount();
            ViewBag.BetCount = await GetBetCount();
            ViewBag.Analytics = await GetAnalytics();

            ViewBag.NextGame = db.Games.Include(j => j.Local).Include(j => j.Visitor).Where<Game>(m => m.StartDate > DateTime.UtcNow).OrderBy<Game, DateTime>(k => k.StartDate).First<Game>();

            return View();
        }

        private async Task<int> GetUserCount()
        {
            if (HttpRuntime.Cache[CacheEnum.UserCount] != null)
                return (int)HttpRuntime.Cache[CacheEnum.UserCount];

            int userCount = await db.Users.CountAsync<ApplicationUser>();
            HttpRuntime.Cache.Insert(CacheEnum.UserCount, userCount, null, DateTime.UtcNow.AddMinutes(15), Cache.NoSlidingExpiration);
            return userCount;
        }

        private async Task<int> GetRemainingGameCount()
        {
            if (HttpRuntime.Cache[CacheEnum.RemainingGameCount] != null)
                return (int)HttpRuntime.Cache[CacheEnum.RemainingGameCount];

            int gameCount = await db.Games.Where<Game>(g => g.StartDate > DateTime.UtcNow).CountAsync<Game>();

            HttpRuntime.Cache.Insert(CacheEnum.RemainingGameCount, gameCount, null, DateTime.UtcNow.AddMinutes(60), Cache.NoSlidingExpiration);
            return gameCount;
        }

        private async Task<int> GetBetCount()
        {
            if (HttpRuntime.Cache[CacheEnum.BetCount] != null)
                return (int)HttpRuntime.Cache[CacheEnum.BetCount];

            int betCount = await db.Bets.CountAsync<Bet>();

            HttpRuntime.Cache.Insert(CacheEnum.BetCount, betCount, null, DateTime.UtcNow.AddMinutes(15), Cache.NoSlidingExpiration);
            return betCount;
        }

        private async Task<AnalyticsReport> GetAnalytics()
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["GoogleAnalytics"] != "Yes")
                    return new AnalyticsReport { PageViews = "Disabled", Sessions = "Disabled" };

                if (HttpRuntime.Cache[CacheEnum.GooglePageViews] != null)
                    return (AnalyticsReport)HttpRuntime.Cache[CacheEnum.GooglePageViews];

                string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "061868b5357ec57ce2cd01f7cba0d45c780d07f6-privatekey.p12");
                var certificate = new X509Certificate2(fileName, "notasecret", X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet);

                ServiceAccountCredential credential = new ServiceAccountCredential(
                   new ServiceAccountCredential.Initializer("976986011558-sitrrlg8ji58a1ad5k9c9feh8p4u6gbh@developer.gserviceaccount.com")
                   {
                       Scopes = new[] { AnalyticsService.Scope.AnalyticsReadonly }
                   }.FromCertificate(certificate));

                using (Google.Apis.Analytics.v3.AnalyticsService service = new Google.Apis.Analytics.v3.AnalyticsService(new BaseClientService.Initializer
                {
                    HttpClientInitializer = credential,
                    ApplicationName = "NSWC2014"
                }))
                {

                    var request = service.Data.Ga.Get("ga:87109047", DateTime.UtcNow.AddDays(-1).ToString("yyyy-MM-dd"), DateTime.UtcNow.ToString("yyyy-MM-dd"), "ga:pageviews,ga:sessions");
                    var result = await request.ExecuteAsync();

                    var report = new AnalyticsReport { PageViews = result.TotalsForAllResults["ga:pageviews"], Sessions = result.TotalsForAllResults["ga:sessions"] };

                    HttpRuntime.Cache.Insert(CacheEnum.GooglePageViews, report, null, DateTime.UtcNow.AddMinutes(60), Cache.NoSlidingExpiration);
                    return report;
                }
            }
            catch
            {
                return new AnalyticsReport { PageViews = "Pending..", Sessions = "Pending.." };
            }
        }

        public class AnalyticsReport
        {
            public string PageViews { get; set; }
            public string Sessions { get; set; }
        }

        [AllowAnonymous]
        public ActionResult Rules()
        {
            return View();
        }

        [AllowAnonymous]
        public ActionResult Error()
        {
            return View();
        }

        [Authorize]
        public ActionResult Chat()
        {
            return View();
        }

        public void SetLastOdds()
        {
            //ignoring games that have already been played
            foreach (Game game in db.Games.Include(j => j.Local).Include(j => j.Visitor).Where<Game>(g => g.StartDate > DateTime.UtcNow))
            {
                Models.Betclic.Match match = games[game.DisplayName + " - " + game.StartDate.ToShortDateString()];

                if (match != null)
                {
                    //Taking the match result
                    game.WinOdd = match.bets.First(b => b.code == "Ftb_Mr3").choice.First(c => c.name == "%1%").odd;
                    game.DrawOdd = match.bets.First(b => b.code == "Ftb_Mr3").choice.First(c => c.name == "Draw").odd;
                    game.LossOdd = match.bets.First(b => b.code == "Ftb_Mr3").choice.First(c => c.name == "%2%").odd;
                }
            }

            db.SaveChanges();
        }

        private WeatherInfo GetWeatherInfo()
        {
            try
            {
                if (HttpRuntime.Cache[CacheEnum.WeatherInfo] != null)
                    return (WeatherInfo)HttpRuntime.Cache[CacheEnum.WeatherInfo];

                lock (_syncRootWeather)
                {
                    string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "weather.json");
                    string backupFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "weather_backup.json");

                    //File already exists and requires update
                    if (System.IO.File.Exists(fileName) && System.IO.File.GetLastWriteTimeUtc(fileName) < DateTime.UtcNow.AddMinutes(-30))
                    {
                        //Removing old backup
                        if (System.IO.File.Exists(backupFileName))
                            System.IO.File.Delete(backupFileName);

                        //moving newer file to backup
                        System.IO.File.Move(fileName, backupFileName);
                    }

                    //File didn't exist or has been moved to backup
                    if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PROD")
                    {
                        if (!System.IO.File.Exists(fileName))
                        {
                            using (WebClient client = new WebClient())
                            {
                                client.DownloadFile("http://api.wunderground.com/api/fca502fbc6701138/forecast/q/FR/Paris.json", fileName);
                            }
                        }
                    }

                    if (System.IO.File.Exists(fileName))
                    {
                        var weatherData = JsonConvert.DeserializeObject<NetsizeWorldCup.Models.WeatherApi.Rootobject>(System.IO.File.ReadAllText(fileName));

                        WeatherInfo winfo = new WeatherInfo { ImageUrl = weatherData.forecast.txt_forecast.forecastday.First().icon_url, Text = weatherData.forecast.txt_forecast.forecastday.First().fcttext_metric };
                        HttpRuntime.Cache.Insert(CacheEnum.WeatherInfo, winfo, null, DateTime.UtcNow.AddMinutes(20), Cache.NoSlidingExpiration);

                        return winfo;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
            catch
            {
                return null;
            }



        }

        private List<FeedItem> GetLastFeeds()
        {
            if (HttpRuntime.Cache[CacheEnum.LastFeeds] != null)
                return (List<FeedItem>)HttpRuntime.Cache[CacheEnum.LastFeeds];

            try
            {
                lock (_syncRootFeed)
                {
                    if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PROD")
                    {
                        string url = "http://www.fifa.com/worldcup/news/rss.xml";
                        SyndicationFeed feed = null;

                        using (XmlReader reader = XmlReader.Create(url))
                        {
                            feed = SyndicationFeed.Load(reader);
                        }

                        List<FeedItem> feeds = feed.Items.Take<SyndicationItem>(5).Select<SyndicationItem, FeedItem>(
                            j => new FeedItem
                            {
                                Title = j.Title.Text,
                                Summary = j.Summary.Text,
                                PublishedDate = j.PublishDate.LocalDateTime.GetPrettyDate(),
                                Url = j.Links[0].GetAbsoluteUri().ToString(),
                                ImageUrl = j.Links[1].GetAbsoluteUri().ToString()
                            }).ToList<FeedItem>();

                        HttpRuntime.Cache.Insert(CacheEnum.LastFeeds, feeds, null, DateTime.UtcNow.AddMinutes(20), Cache.NoSlidingExpiration);

                        return feeds;
                    }
                    else
                    {
                        return new List<FeedItem>();
                    }
                }
            }
            catch
            {
                return new List<FeedItem>();
            }

        }


        private string GetLastOdds()
        {
            try
            {
                NetsizeWorldCup.Models.Betclic.sports result = null;
                bool fileUpdated = true;

                string fileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "odds.xml");
                string backupFileName = System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "App_Data", "odds_backup.xml");

                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] == "PROD")
                {
                    //File already exists and requires update
                    if (System.IO.File.Exists(fileName) && System.IO.File.GetLastWriteTimeUtc(fileName) < DateTime.UtcNow.AddMinutes(-15))
                    {
                        fileUpdated = true;

                        //Removing old backup
                        if (System.IO.File.Exists(backupFileName))
                            System.IO.File.Delete(backupFileName);

                        //moving newer file to backup
                        System.IO.File.Move(fileName, backupFileName);
                    }

                    //File didn't exist or has been moved to backup
                    if (!System.IO.File.Exists(fileName))
                    {
                        try
                        {
                            GetOdds(fileName);
                        }
                        catch (System.IO.IOException ioEx)
                        {
                            GetOdds(fileName);
                        }
                    }
                }

                //File has been correctly downloaded
                if (System.IO.File.Exists(fileName))
                {
                    XmlSerializer serializer = new XmlSerializer(typeof(NetsizeWorldCup.Models.Betclic.sports));

                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        result = (NetsizeWorldCup.Models.Betclic.sports)serializer.Deserialize(reader);
                    }

                    var sport = result.sport.First(s => s.name == "Football");
                    var wcEvent = sport.@event.First(e => e.name == "World Cup");

                    games = wcEvent.match.ToDictionary<Models.Betclic.Match, string>(i => i.name + " - " + i.start_date.AddHours(-1).ToShortDateString());

                    //We had an update so we need to reset the db
                    if (fileUpdated)
                        SetLastOdds();

                    return "Odds updated " + result.file_date.AddHours(1).GetPrettyDate().ToString();
                }
                else
                {
                    return "failure";
                }
            }
            catch
            {
                return "failure";
            }
        }

        public static void GetOdds(string filename)
        {
            using (WebClient client = new WebClient())
            {
                client.DownloadFile("http://xml.cdn.betclic.com/odds_en.xml", filename);
            }
        }

        [AllowAnonymous]
        public JsonResult UpdateOdds(string password)
        {
            if (password!="orange05!")
                return Json(new { status = false });

            try
            {
                this.GetLastOdds();
                return Json(new { status = true }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { status = false, message = ex.ToString() });
            }
        }

        [AllowAnonymous]
        public JsonResult UpdateResults(string password)
        {
            if (password != "orange05!")
                return Json(new { status = false });

            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["UpdateResults"] == "Yes")
                {
                    string url = "http://www.scorespro.com/rss2/live-soccer.xml";
                    SyndicationFeed feed = null;

                    using (XmlReader reader = XmlReader.Create(url))
                    {
                        feed = SyndicationFeed.Load(reader);
                    }

                    if (feed == null)
                        return Json(new { Result = false, Message = "Feed is null" }, JsonRequestBehavior.AllowGet);

                    var modified = false;

                    foreach (var item in feed.Items)
                    {
                        if (item.Title.Text.Contains("(FIFA-GS)"))
                        {
                            string data = item.Title.Text.Substring(item.Title.Text.IndexOf("(FIFA-GS)") + 9).Trim();
                            string gameInfo = data.Substring(0, data.IndexOf(":"));

                            string score = data.Substring(data.IndexOf(":") + 1).Trim();
                            int scoreLocal = Convert.ToInt32(score.Split('-')[0]);
                            int scoreVisitor = Convert.ToInt32(score.Split('-')[1]);

                            var gameResult = 0;

                            if (scoreLocal > scoreVisitor)
                                gameResult = 1;
                            else if (scoreLocal == scoreVisitor)
                                gameResult = 2;
                            else
                                gameResult = 3;

                            string local = gameInfo.Split(new string[] { " vs " }, StringSplitOptions.None)[0].Trim();
                            string visitor = gameInfo.Split(new string[] { " vs " }, StringSplitOptions.None)[1].Trim();

                            //coming back to UTC
                            DateTime publishedDate = item.PublishDate.UtcDateTime;

                            Game game = db.Games.ToList<Game>().FirstOrDefault<Game>(g => g.Local.Name == local && g.Visitor.Name == visitor && g.StartDate.ToShortDateString() == publishedDate.ToShortDateString());

                            if (game == null)
                                continue;

                            if (game.Result != gameResult)
                            { 
                                game.Result = gameResult;
                                modified = true;
                            }
                        }
                        //"#Soccer #Livescore @ScoresPro: (FIFA-GS) Spain vs Netherlands: 0-0"
                    }

                    if (modified)
                    {
                        db.SaveChanges();

                        HttpRuntime.Cache.Remove(CacheEnum.Scores);
                        AccountController.ComputeScores(this.db);
                    }

                    return Json(new { Result = true }, JsonRequestBehavior.AllowGet);
                }

                return Json(new { Result = false, Message = "Update results disabled" }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { Result = false, Message = ex.ToString() }, JsonRequestBehavior.AllowGet);
            }
        }
    }

    public class FeedItem
    {
        public string Title { get; set; }
        public string Summary { get; set; }
        public string ImageUrl { get; set; }
        public string PublishedDate { get; set; }
        public string Url { get; set; }
    }

    public class WeatherInfo
    {
        public string ImageUrl { get; set; }
        public string Text { get; set; }
    }
}