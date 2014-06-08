using NetsizeWorldCup.Models;
using System.Linq;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;

namespace NetsizeWorldCup.Controllers
{
    public class BetController : BaseController
    {
        // POST: Bet/Create
        [HttpPost]
        [Authorize]
        public JsonResult Place(PlaceBetModel placedBet)
        {
            if (!User.Identity.IsAuthenticated)
                return Json(new { Status = false });

            var user = UserManager.FindById(User.Identity.GetUserId());
            var game = db.Games.FirstOrDefault<Game>(g => g.ID == placedBet.GameId);

            if (game == null)
                return Json(new { Status = false });

            Bet bet = db.Bets.FirstOrDefault<Bet>(b => b.Owner.Id == user.Id && b.Game.ID == game.ID);

            if (bet == null)
            {
                db.Bets.Add(new Bet { Owner = user, Forecast = placedBet.Result, Game = game });
            }
            else
            {
                bet.Forecast = placedBet.Result;
            }

            db.SaveChanges();

            return Json(new { Status = true });
        }
    }
}
