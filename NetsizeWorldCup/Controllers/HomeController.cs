using NetsizeWorldCup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Data.Entity;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Net;
using System.Xml.Serialization;
using System.IO;
using Newtonsoft.Json;
using Microsoft.AspNet.Identity;

namespace NetsizeWorldCup.Controllers
{
    public class HomeController : BaseController
    {
        public HomeController()
            : base()
        { }

        Dictionary<string, Models.Betclic.Match> games;

        public ActionResult Index()
        {
            string currentUserId = User.Identity.GetUserId();
            var user = db.Users.FirstOrDefault<ApplicationUser>(u => u.Id == currentUserId);

            if (user != null)
                ViewBag.CurrentTimeZoneInfo = user.TimeZoneInfo;
            else
                ViewBag.CurrentTimeZoneInfo = TimeZoneInfo.Local;

            ViewBag.Feeds = GetLastFeeds();
            //ViewBag.Test = GetLastOdds();
            ViewBag.WeatherInfo = GetWeatherInfo();

            ViewBag.NextGame = db.Games.Include(j => j.Local).Include(j => j.Visitor).Where<Game>(m => m.StartDate > DateTime.UtcNow).OrderBy<Game, DateTime>(k => k.StartDate).First<Game>();

            return View();
        }

        public void SetLastOdds()
        {
            foreach (Game game in db.Games.Include(j => j.Local).Include(j => j.Visitor))
            {
                Models.Betclic.Match match = games[game.DisplayName];

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

        public WeatherInfo GetWeatherInfo()
        {
            try
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
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] != "DEV")
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
                    return winfo;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        private List<FeedItem> GetLastFeeds()
        {
            try
            {
                if (System.Configuration.ConfigurationManager.AppSettings["Environment"] != "DEV")
                {
                    string url = "http://www.fifa.com/worldcup/news/rss.xml";
                    SyndicationFeed feed = null;

                    using (XmlReader reader = XmlReader.Create(url))
                    {
                        feed = SyndicationFeed.Load(reader);
                    }

                    return feed.Items.Take<SyndicationItem>(5).Select<SyndicationItem, FeedItem>(
                        j => new FeedItem
                        {
                            Title = j.Title.Text,
                            Summary = j.Summary.Text,
                            PublishedDate = j.PublishDate.LocalDateTime.GetPrettyDate(),
                            Url = j.Links[0].GetAbsoluteUri().ToString(),
                            ImageUrl = j.Links[1].GetAbsoluteUri().ToString()
                        }).ToList<FeedItem>();
                }
                else
                {
                    return new List<FeedItem>();
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

                    games = wcEvent.match.ToDictionary<Models.Betclic.Match, string>(i => i.name);

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
        public JsonResult UpdateOdds()
        {
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