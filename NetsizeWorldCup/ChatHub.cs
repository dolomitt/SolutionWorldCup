using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.AspNet.SignalR;
using NetsizeWorldCup.Models;
using NetsizeWorldCup.Controllers;
using System.Text.RegularExpressions;

namespace NetsizeWorldCup
{
    public class ChatHub : Hub
    {
        static object syncRoot = new object();

        public void Send(string pic, string message)
        {
            message = CleanInput(message);

            if (!String.IsNullOrEmpty(Context.User.Identity.Name))
            {
                using (ApplicationDbContext db = new ApplicationDbContext())
                {
                    var user = db.Users.First<ApplicationUser>(u => u.UserName == Context.User.Identity.Name);

                    if (user != null)
                    {
                        var newMessage = new Message { Body = message, Owner = user };

                        db.Messages.Add(newMessage);
                        db.SaveChanges();

                        // Call the addNewMessageToPage method to update clients.
                        Clients.All.addNewMessageToPage(Context.User.Identity.Name, pic, CleanInput(message), newMessage.CreationDate);
                    }
                }
            }
        }

        static string CleanInput(string strIn)
        {
            // Replace invalid characters with empty strings.
            try
            {
                return Regex.Replace(strIn, @"[^#\w\s\.@-]", "",
                                     RegexOptions.None, TimeSpan.FromSeconds(1.5));
            }
            // If we timeout when replacing invalid characters, 
            // we should return Empty.
            catch (RegexMatchTimeoutException)
            {
                return String.Empty;
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