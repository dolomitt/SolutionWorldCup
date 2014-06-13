using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using NetsizeWorldCup.Models;

namespace NetsizeWorldCup
{
    public class ChatHub : Hub
    {
        public void Send(string name, string pic, string message)
        {
            // Call the addNewMessageToPage method to update clients.
            Clients.All.addNewMessageToPage(Context.User.Identity.Name, pic, message);

            using (ApplicationDbContext db = new ApplicationDbContext())
            {
                var user = db.Users.First<ApplicationUser>(u => u.UserName == Context.User.Identity.Name);

                db.Messages.Add(new Message { Body = message, Owner = user });
                db.SaveChangesAsync();
            }
        }
    }
}