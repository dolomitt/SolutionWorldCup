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

namespace NetsizeWorldCup.Controllers
{
    public class HomeController : BaseController
    {
        Dictionary<string, Models.Betclic.Match> games;

        public ActionResult Index()
        {
            ViewBag.Feeds = GetLastFeeds();
            ViewBag.Test = GetLastOdds();


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

        public List<FeedItem> GetLastFeeds()
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


        public string GetLastOdds()
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
                using (WebClient client = new WebClient())
                {
                    client.DownloadFile("http://xml.cdn.betclic.com/odds_en.xml", fileName);
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

                return "Game Count = " + wcEvent.match.Count() + " - Odds updated at " + result.file_date.AddHours(1).ToString();
            }
            else
            {
                return "failure";
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
}