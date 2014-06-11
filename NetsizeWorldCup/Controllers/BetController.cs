using NetsizeWorldCup.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using System.Collections.Concurrent;

namespace NetsizeWorldCup.Controllers
{
    public class BetController : BaseController
    {
        // globally declare a map of session id to mutexes
        static ConcurrentDictionary<string, object> mutexMap = new ConcurrentDictionary<string, object>();

        public BetController()
            : base()
        {

        }

        // POST: Bet/Create
        [HttpPost]
        [Authorize]
        public JsonResult Place(PlaceBetModel placedBet)
        {
            try
            {
                if (!User.Identity.IsAuthenticated)
                    return Json(new { Status = false });

                string currentUserId = User.Identity.GetUserId();
                var user = db.Users.First<ApplicationUser>(u => u.Id == currentUserId);

                // now you can aquire a lock per session as follows
                object mutex = mutexMap.GetOrAdd(HttpContext.Session.SessionID, key => new object());

                lock (mutex)
                {
                    // Do stuff with the connection
                    var game = db.Games.FirstOrDefault<Game>(g => g.ID == placedBet.GameId);

                    if (game == null)
                        return Json(new { Status = false, Message = "Game does not exist" });

                    //game start date is past
                    if (game.StartDate < DateTime.UtcNow || game.Result.HasValue)
                        return Json(new { Status = false, Message = "Game already started" });

                    Bet bet = db.Bets.FirstOrDefault<Bet>(b => b.Owner.Id == user.Id && b.Game.ID == game.ID);

                    if (bet == null)
                    {
                        Bet newBet = new Bet { Owner = user, Forecast = placedBet.Result, Game = game, WinOdd = game.WinOdd, DrawOdd = game.DrawOdd, LossOdd = game.LossOdd };
                        db.Bets.Add(newBet);
                    }
                    else
                    {
                        bet.Forecast = placedBet.Result;

                        bet.WinOdd = game.WinOdd;
                        bet.DrawOdd = game.DrawOdd;
                        bet.LossOdd = game.LossOdd;
                    }

                    db.SaveChanges();
                }

                return Json(new { Status = true });
            }
            catch
            {
                return Json(new { Status = false, Message = "An error occured, please refresh your page" });
                throw;
            }
        }
    }
}
