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

namespace NetsizeWorldCup.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            string url = "http://www.fifa.com/worldcup/news/rss.xml";
            SyndicationFeed feed = null;

            using (XmlReader reader = XmlReader.Create(url))
            {
                feed = SyndicationFeed.Load(reader);
            }

            ViewBag.Feeds = feed.Items.Take<SyndicationItem>(5).Select<SyndicationItem, FeedItem>(
                j => new FeedItem 
                { 
                    Title = j.Title.Text, 
                    Summary = j.Summary.Text,
                    PublishedDate = j.PublishDate.LocalDateTime.GetPrettyDate(),
                    Url = j.Links[0].GetAbsoluteUri().ToString(),
                    ImageUrl = j.Links[1].GetAbsoluteUri().ToString()
                }).ToList<FeedItem>();

            ViewBag.NextGame = db.Games.Include(j => j.Local).Include(j => j.Visitor).Where<Game>(m => m.StartDate > DateTime.UtcNow).OrderBy<Game,DateTime>(k=>k.StartDate).First<Game>();

            return View();
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