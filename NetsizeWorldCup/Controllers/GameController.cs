﻿using System;
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
using Microsoft.AspNet.Identity;

namespace NetsizeWorldCup.Controllers
{
    public class GameController : BaseController
    {
        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: Game
        public async Task<ActionResult> Index()
        {
            ViewBag.UserId = User.Identity.GetUserId();
            var user = this.UserManager.FindById<ApplicationUser, string>(User.Identity.GetUserId());

            if (user != null)
                ViewBag.CurrentTimeZoneInfo = user.TimeZoneInfo;
            else
                ViewBag.CurrentTimeZoneInfo = TimeZoneInfo.Local;

            return View(await db.Games.OrderBy<Game,DateTime>(j=>j.StartDate).ToListAsync());
        }

        public async Task<ActionResult> Calendar()
        {
            return View(await db.Games.OrderBy<Game, DateTime>(j => j.StartDate).ToListAsync());
        }

        public JsonResult GetCalendarEvents()
        {
            var user = this.UserManager.FindById<ApplicationUser, string>(User.Identity.GetUserId());
            List<CalendarEvent> events = new List<CalendarEvent>();
            TimeZoneInfo tzi = TimeZoneInfo.Local;

            if (user != null)
                tzi = user.TimeZoneInfo;

            foreach(Game i in db.Games.Include(j=>j.Local).Include(j=>j.Visitor))
                events.Add(new CalendarEvent { start = i.StartDate.UtcToLocal(tzi).ToString("yyyy-MM-dd HH:mm"), end = i.EndDate.UtcToLocal(tzi).ToString("yyyy-MM-dd HH:mm"), title = i.DisplayName, allDay = false });

            return Json(events, JsonRequestBehavior.AllowGet); 
        }

        // GET: Game/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = await db.Games.FindAsync(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // GET: Game/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Game/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "ID,StartDate,Location,Result,CreationDate")] Game game)
        {
            if (ModelState.IsValid)
            {
                db.Games.Add(game);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(game);
        }

        // GET: Game/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = await db.Games.FindAsync(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // POST: Game/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "ID,StartDate,Location,Result,CreationDate")] Game game)
        {
            if (ModelState.IsValid)
            {
                db.Entry(game).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(game);
        }

        // GET: Game/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Game game = await db.Games.FindAsync(id);
            if (game == null)
            {
                return HttpNotFound();
            }
            return View(game);
        }

        // POST: Game/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Game game = await db.Games.FindAsync(id);
            db.Games.Remove(game);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }

    public class CalendarEvent
    {
        public string title { get; set; }
        public string start { get; set; }
        public string end { get; set; }
        public bool allDay { get; set; }
    }
}