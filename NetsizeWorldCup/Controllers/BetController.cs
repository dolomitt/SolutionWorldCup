using NetsizeWorldCup.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System;

namespace NetsizeWorldCup.Controllers
{
    public class BetController : BaseController
    {
        public BetController()
            : base()
        {

        }

        // POST: Bet/Create
        [HttpPost]
        [Authorize]
        public JsonResult Place(PlaceBetModel placedBet)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { Status = false });

            string currentUserId = User.Identity.GetUserId();

            var user = db.Users.First<ApplicationUser>(u => u.Id == currentUserId);
            var game = db.Games.FirstOrDefault<Game>(g => g.ID == placedBet.GameId);

            if (game == null)
                return Json(new { Status = false });

            //game start date is past
            if (game.StartDate < DateTime.UtcNow)
                return Json(new { Status = false });

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

            return Json(new { Status = true });
        }
    }
}
