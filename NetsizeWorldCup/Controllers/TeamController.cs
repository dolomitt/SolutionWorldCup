using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using NetsizeWorldCup;
using NetsizeWorldCup.Models;
using System.Web.Caching;

namespace NetsizeWorldCup.Controllers
{
    public class TeamController : BaseController
    {
        public TeamController()
            : base()
        {
        }

        private List<TeamModel> GetTeams()
        {
            if (HttpRuntime.Cache[CacheEnum.TeamList] != null)
                return (List<TeamModel>)HttpRuntime.Cache[CacheEnum.TeamList];

            List<TeamModel> teams = db.Teams.OrderBy<Team, string>(t => t.Group.Name + "_" + t.Name).Select<Team, TeamModel>(t => new TeamModel { FlagUrl = t.FlagUrl, ID = t.ID, Name = t.Name, Group = t.Group.Name }).ToList<TeamModel>();

            HttpRuntime.Cache.Insert(CacheEnum.TeamList, teams, null, DateTime.UtcNow.AddMinutes(30), Cache.NoSlidingExpiration);
            return teams;
        }

        // GET: Team
        public ActionResult Index()
        {
            return View(GetTeams());
        }

        //// GET: Team/Details/5
        //public async Task<ActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Team team = await db.Teams.FindAsync(id);
        //    if (team == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(team);
        //}

        //// GET: Team/Create
        //public ActionResult Create()
        //{
        //    return View();
        //}

        //// POST: Team/Create
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Create([Bind(Include = "ID,FlagUrl,Name,CreationDate")] Team team)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Teams.Add(team);
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }

        //    return View(team);
        //}

        //// GET: Team/Edit/5
        //public async Task<ActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Team team = await db.Teams.FindAsync(id);
        //    if (team == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(team);
        //}

        //// POST: Team/Edit/5
        //// To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        //// more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> Edit([Bind(Include = "ID,FlagUrl,Name,CreationDate")] Team team)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        db.Entry(team).State = EntityState.Modified;
        //        await db.SaveChangesAsync();
        //        return RedirectToAction("Index");
        //    }
        //    return View(team);
        //}

        //// GET: Team/Delete/5
        //public async Task<ActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }
        //    Team team = await db.Teams.FindAsync(id);
        //    if (team == null)
        //    {
        //        return HttpNotFound();
        //    }
        //    return View(team);
        //}

        //// POST: Team/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<ActionResult> DeleteConfirmed(int id)
        //{
        //    Team team = await db.Teams.FindAsync(id);
        //    db.Teams.Remove(team);
        //    await db.SaveChangesAsync();
        //    return RedirectToAction("Index");
        //}

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
