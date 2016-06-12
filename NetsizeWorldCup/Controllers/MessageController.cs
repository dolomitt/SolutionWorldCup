using Microsoft.AspNet.Identity;
using NetsizeWorldCup.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace NetsizeWorldCup.Controllers
{
    public class MessageController : BaseController
    {
        // GET: Message
        [Authorize]
        public JsonResult GetLastMessages()
        {
            try
            {
                var messages = Json(
                    new
                    {
                        Messages = db.Messages
                        .OrderByDescending<Message, int>(m => m.ID)
                        .Take<Message>(20).ToList().Select<Message, MessageModel>(m => new MessageModel
                        {
                            ID = m.ID,
                            Name = m.Owner.UserName,
                            PictureUrl = m.Owner.ImageUrl,
                            Message = m.Body,
                            Date = GetPrettyDate(m.CreationDate)
                        })
                        .OrderBy<MessageModel, int>(m => m.ID).ToList<MessageModel>()
                    }, JsonRequestBehavior.AllowGet);

                return messages;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return null;
            }
        }

        static string GetPrettyDate(DateTime d)
        {
            // 1.
            // Get time span elapsed since the date.
            TimeSpan s = DateTime.UtcNow.Subtract(d);

            // 2.
            // Get total number of days elapsed.
            int dayDiff = (int)s.TotalDays;

            // 3.
            // Get total number of seconds elapsed.
            int secDiff = (int)s.TotalSeconds;

            // 4.
            // Don't allow out of range values.
            if (dayDiff < 0 || dayDiff >= 31)
            {
                return null;
            }

            // 5.
            // Handle same-day times.
            if (dayDiff == 0)
            {
                // A.
                // Less than one minute ago.
                if (secDiff < 60)
                {
                    return "just now";
                }
                // B.
                // Less than 2 minutes ago.
                if (secDiff < 120)
                {
                    return "1 min ago";
                }
                // C.
                // Less than one hour ago.
                if (secDiff < 3600)
                {
                    return string.Format("{0} min ago",
                        Math.Floor((double)secDiff / 60));
                }
                // D.
                // Less than 2 hours ago.
                if (secDiff < 7200)
                {
                    return "1 hour ago";
                }
                // E.
                // Less than one day ago.
                if (secDiff < 86400)
                {
                    return string.Format("{0} hours ago",
                        Math.Floor((double)secDiff / 3600));
                }
            }
            // 6.
            // Handle previous days.
            if (dayDiff == 1)
            {
                return "yesterday";
            }
            if (dayDiff < 7)
            {
                return string.Format("{0} days ago",
                dayDiff);
            }
            if (dayDiff < 31)
            {
                return string.Format("{0} weeks ago",
                Math.Ceiling((double)dayDiff / 7));
            }
            return null;
        }

        public class MessageModel
        {
            public string Name { get; set; }
            public string PictureUrl { get; set; }
            public string Message { get; set; }
            public int ID { get; set; }

            public string Date { get; set; }
        }
    }
}