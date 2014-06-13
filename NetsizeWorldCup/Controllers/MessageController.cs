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
            return Json(
                new { 
                    Messages = db.Messages
                    .OrderByDescending<Message, int>(m => m.ID)
                    .Take<Message>(20).Select<Message, MessageModel>(m => new MessageModel { Name = m.Owner.UserName, PictureUrl = m.Owner.ImageUrl, Message = m.Body })
                    .ToList<MessageModel>() }, JsonRequestBehavior.AllowGet);
        }

        public class MessageModel
        {
            public string Name { get; set; }
            public string PictureUrl { get; set; }
            public string Message { get; set; }
        }
    }
}