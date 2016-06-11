using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using NetsizeWorldCup.Models;
using NetsizeWorldCup.Controllers;

namespace NetsizeWorldCup
{
    public class ChatHub : Hub
    {
        static object syncRoot = new object();

        public void Send(string pic, string message)
        {
            if (!String.IsNullOrEmpty(Context.User.Identity.Name))
            {
                // Call the addNewMessageToPage method to update clients.
                Clients.All.addNewMessageToPage(Context.User.Identity.Name, pic, message);

                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var user = db.Users.First<ApplicationUser>(u => u.UserName == Context.User.Identity.Name);

                    if (user != null)
                    {
                        db.Messages.Add(new Message { Body = message, Owner = user });
                        db.SaveChanges();
                    }
                }
            }
        }

        public override System.Threading.Tasks.Task OnConnected()
        {
            lock (syncRoot)
            {
                if (HttpRuntime.Cache[CacheEnum.ConnectedPlayers] == null)
                    HttpRuntime.Cache[CacheEnum.ConnectedPlayers] = new List<string>();

                List<string> connectedPlayers = (List<string>)HttpRuntime.Cache[CacheEnum.ConnectedPlayers];

                if (!connectedPlayers.Contains(Context.User.Identity.Name))
                    connectedPlayers.Add(Context.User.Identity.Name);

                return base.OnConnected();
            }
        }

        public override System.Threading.Tasks.Task OnDisconnected()
        {
            lock (syncRoot)
            {
                if (HttpRuntime.Cache[CacheEnum.ConnectedPlayers] == null)
                    HttpRuntime.Cache[CacheEnum.ConnectedPlayers] = new List<string>();

                List<string> connectedPlayers = (List<string>)HttpRuntime.Cache[CacheEnum.ConnectedPlayers];

                if (connectedPlayers.Contains(Context.User.Identity.Name))
                    connectedPlayers.Remove(Context.User.Identity.Name);

                return base.OnDisconnected();
            }
        }
    }
}