using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        ShopAppEntities db=new ShopAppEntities();
        public ActionResult Index(int sayfa=1)
        {
            var urunler = db.tblUrunler.ToList().OrderByDescending(x =>x.id).ToPagedList(sayfa, 8);
            return View(urunler);
        }

        public ActionResult UrunKategoriPartial()
        {
            return PartialView(db.tblKategoriler.Include("tblUrunler").ToList().OrderBy(x => x.ad));
        }
        
        public ActionResult UrunIncele(int id)
        {
            var urun = db.tblUrunler.Find(id);
            return View(urun);
        }
    }
}