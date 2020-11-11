using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        ShopAppEntities db=new ShopAppEntities();
        [Route("")]
        [Route("home")]
        public ActionResult Index(int sayfa=1)
        {
            var urunler = db.tblUrunler.ToList().OrderByDescending(x =>x.id).ToPagedList(sayfa, 8);
            return View(urunler);
        }

        public ActionResult UrunKategoriPartial()
        {
            return PartialView(db.tblKategoriler.Include("tblUrunler").ToList().OrderBy(x => x.ad));
        }
        [Route("home/urunincele/{ad}-{id:int}")]
        public ActionResult UrunIncele(int id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblUrunler p = db.tblUrunler.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }

            return View(p);
        }

        public ActionResult SliderPartial()
        {
            return PartialView(db.tblSlider.ToList().OrderByDescending(x => x.id));
        }

        public ActionResult FooterPartial()
        {
            @ViewBag.Iletisim = db.tblIletisim.SingleOrDefault();
            @ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            @ViewBag.Kategoriler=db.tblKategoriler.ToList().OrderBy(x =>x.ad);
            return PartialView();
        }
        [Route("home/kategori/{ad}-{id:int}")]
        public ActionResult Kategori(int id,int sayfa = 1)
        {
            var kategori = db.tblUrunler.Include("tblKategoriler").OrderByDescending(x =>x.id).Where(x => x.tblKategoriler.id == id).ToList()
                .ToPagedList(sayfa, 12);
            return View(kategori);
        }

        public ActionResult SiparisKaydi(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblSiparisKaydi p = db.tblSiparisKaydi.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }

            return View(p);
        }

        public ActionResult KayitOl()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KayitOl(tblMusteriler p,HttpPostedFileBase resimUrl)
        {
            if (resimUrl != null)
            {
                WebImage img = new WebImage(resimUrl.InputStream);
                FileInfo imginfo = new FileInfo(resimUrl.FileName);

                string profilimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                img.Resize(300, 300);
                img.Save("~/Uploads/Profil/" + profilimgname);

                p.resimUrl = "/Uploads/Profil/" + profilimgname;
            }
            db.tblMusteriler.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index","Giris");
        }

        public ActionResult Profil(int id )
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblMusteriler p = db.tblMusteriler.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }

            return View(p);
        }

        
    }
}