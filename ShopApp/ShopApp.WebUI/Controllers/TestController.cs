using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class TestController : Controller
    {
        private ShopAppEntities db = new ShopAppEntities();

        // GET: Test
        public ActionResult Index()
        {
            var tblCesitler = db.tblCesitler.Include(t => t.tblKategoriler);
            return View(tblCesitler.ToList());
        }

        // GET: Test/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCesitler tblCesitler = db.tblCesitler.Find(id);
            if (tblCesitler == null)
            {
                return HttpNotFound();
            }
            return View(tblCesitler);
        }

        // GET: Test/Create
        public ActionResult Create()
        {
            ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad");
            return View();
        }

        // POST: Test/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "id,ad,aciklama,resimUrl,kategori")] tblCesitler tblCesitler)
        {
            if (ModelState.IsValid)
            {
                db.tblCesitler.Add(tblCesitler);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad", tblCesitler.kategori);
            return View(tblCesitler);
        }

        // GET: Test/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCesitler tblCesitler = db.tblCesitler.Find(id);
            if (tblCesitler == null)
            {
                return HttpNotFound();
            }
            ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad", tblCesitler.kategori);
            return View(tblCesitler);
        }

        // POST: Test/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "id,ad,aciklama,resimUrl,kategori")] tblCesitler tblCesitler)
        {
            if (ModelState.IsValid)
            {
                db.Entry(tblCesitler).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad", tblCesitler.kategori);
            return View(tblCesitler);
        }

        // GET: Test/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            tblCesitler tblCesitler = db.tblCesitler.Find(id);
            if (tblCesitler == null)
            {
                return HttpNotFound();
            }
            return View(tblCesitler);
        }

        // POST: Test/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            tblCesitler tblCesitler = db.tblCesitler.Find(id);
            db.tblCesitler.Remove(tblCesitler);
            db.SaveChanges();
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
}
