using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using PagedList;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        private ShopAppEntities db = new ShopAppEntities();

        [Route("admin")]
        public ActionResult Index()
        {

            return View();
        }

        public ActionResult Kategoriler(int sayfa = 1)
        {
            var kategorilistesi = db.tblKategoriler.ToList().ToPagedList(sayfa, 10);
            return View(kategorilistesi);
        }

        public ActionResult KategoriEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriEkle(tblKategoriler p, HttpPostedFileBase resimUrl)
        {
            if (resimUrl != null)
            {
                WebImage img = new WebImage(resimUrl.InputStream);
                FileInfo imginfo = new FileInfo(resimUrl.FileName);

                string ktgimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                img.Resize(800, 500);
                img.Save("~/Uploads/Kategori/" + ktgimgname);

                p.resimUrl = "/Uploads/Kategori/" + ktgimgname;

            }

            db.tblKategoriler.Add(p);
            db.SaveChanges();
            return RedirectToAction("Kategoriler");
        }

        public ActionResult KategoriSil(int id)
        {
            var ktg = db.tblKategoriler.Find(id);
            db.tblKategoriler.Remove(ktg);
            db.SaveChanges();
            return RedirectToAction("Kategoriler");
        }

        [Route("admin/kategoriduzenle/{ad}-{id:int}")]
        [HttpGet]
        public ActionResult KategoriDuzenle(int id)
        {
            var kategoriduzenle = db.tblKategoriler.Where(x => x.id == id).SingleOrDefault();
            return View(kategoriduzenle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KategoriDuzenle(int id, tblKategoriler p, HttpPostedFileBase resimUrl)
        {
            if (ModelState.IsValid)
            {
                var kategoriduzenle = db.tblKategoriler.Where(x => x.id == id).SingleOrDefault();
                if (resimUrl != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(kategoriduzenle.resimUrl)))
                    {
                        System.IO.File.Delete(Server.MapPath(kategoriduzenle.resimUrl));
                    }

                    WebImage img = new WebImage(resimUrl.InputStream);
                    FileInfo imginfo = new FileInfo(resimUrl.FileName);

                    string ktgimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(800, 500);
                    img.Save("~/Uploads/Kategori/" + ktgimgname);

                    kategoriduzenle.resimUrl = "/Uploads/Kategori/" + ktgimgname;
                }

                kategoriduzenle.ad = p.ad;
                kategoriduzenle.aciklama = p.aciklama;
                db.SaveChanges();
                return RedirectToAction("Kategoriler");
            }

            return View(p);

        }

        public ActionResult Cesitler(int sayfa = 1)
        {
            var cesitler = db.tblCesitler.ToList().ToPagedList(sayfa, 10);
            return View(cesitler);
        }

        public ActionResult CesitEkle()
        {
            @ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CesitEkle(tblCesitler p)
        {
            db.tblCesitler.Add(p);
            db.SaveChanges();
            return RedirectToAction("Cesitler");
        }

        public ActionResult CesitSil(int id)
        {
            var cesitsil = db.tblCesitler.Find(id);
            db.tblCesitler.Remove(cesitsil);
            db.SaveChanges();
            return RedirectToAction("Cesitler");
        }

        [Route("admin/cesitduzenle/{ad}-{id:int}")]
        public ActionResult CesitDuzenle(int id)
        {
            var cesitduzenle = db.tblCesitler.Where(x => x.id == id).SingleOrDefault();
            @ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad");
            return View(cesitduzenle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CesitDuzenle(int id, tblCesitler p)
        {

            if (ModelState.IsValid)
            {
                var cesitduzenle = db.tblCesitler.Where(x => x.id == id).SingleOrDefault();
                cesitduzenle.resimUrl = p.resimUrl;
                cesitduzenle.ad = p.ad;
                cesitduzenle.aciklama = p.aciklama;
                cesitduzenle.kategori = p.kategori;
                db.SaveChanges();
                return RedirectToAction("Cesitler");
            }

            return View(p);
        }

        public ActionResult Markalar(int sayfa = 1)
        {
            var markalar = db.tblMarkalar.ToList().ToPagedList(sayfa, 10);
            return View(markalar);
        }

        public ActionResult MarkaEkle()
        {
            @ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad");
            @ViewBag.cesit = new SelectList(db.tblCesitler, "id", "ad");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkaEkle(tblMarkalar p, HttpPostedFileBase resimUrl)
        {
            if (resimUrl != null)
            {
                WebImage img = new WebImage(resimUrl.InputStream);
                FileInfo imginfo = new FileInfo(resimUrl.FileName);

                string markaimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                img.Resize(600, 600);
                img.Save("~/Uploads/Marka/" + markaimgname);

                p.resimUrl = "/Uploads/Marka/" + markaimgname;

            }

            db.tblMarkalar.Add(p);
            db.SaveChanges();
            return RedirectToAction("Markalar");
        }

        public ActionResult MarkaSil(int id)
        {
            var markasil = db.tblMarkalar.Find(id);
            db.tblMarkalar.Remove(markasil);
            db.SaveChanges();
            return RedirectToAction("Markalar");
        }
        [Route("admin/markaduzenle/{ad}-{id:int}")]
        public ActionResult MarkaDuzenle(int id)
        {
            var markaduzenle = db.tblMarkalar.Where(x => x.id == id).SingleOrDefault();
            @ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad");
            @ViewBag.cesit = new SelectList(db.tblCesitler, "id", "ad");
            return View(markaduzenle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkaDuzenle(int id, tblMarkalar p, HttpPostedFileBase resimUrl)
        {
            if (ModelState.IsValid)
            {
                var markaduzenle = db.tblMarkalar.Where(x => x.id == id).SingleOrDefault();
                if (resimUrl != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(markaduzenle.resimUrl)))
                    {
                        System.IO.File.Delete(Server.MapPath(markaduzenle.resimUrl));
                    }

                    WebImage img = new WebImage(resimUrl.InputStream);
                    FileInfo imginfo = new FileInfo(resimUrl.FileName);

                    string markaimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(600, 600);
                    img.Save("~/Uploads/Marka/" + markaimgname);

                    p.resimUrl = "/Uploads/Marka/" + markaimgname;
                }

                markaduzenle.ad = p.ad;
                markaduzenle.cesit = p.cesit;
                markaduzenle.kategori = p.kategori;
                markaduzenle.resimUrl = p.resimUrl;
                db.SaveChanges();
                return RedirectToAction("Markalar");
            }

            return View(p);
        }

        public ActionResult Urunler(int sayfa = 1)
        {
            var urunler = db.tblUrunler.ToList().OrderByDescending(x => x.id).ToPagedList(sayfa, 10);
            return View(urunler);
        }

        public ActionResult UrunEkle()
        {
            @ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad");
            @ViewBag.stok = new SelectList(db.tblStokYonetimi, "id", "durum");
            @ViewBag.marka = new SelectList(db.tblMarkalar, "id", "ad");
            @ViewBag.cesit = new SelectList(db.tblCesitler, "id", "ad");
            return View();
        }
        
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult UrunEkle(tblUrunler p, HttpPostedFileBase resimUrl)
        {

            if (resimUrl != null)
            {
                WebImage img = new WebImage(resimUrl.InputStream);
                FileInfo imginfo = new FileInfo(resimUrl.FileName);

                string urunimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                img.Resize(800, 800);
                img.Save("~/Uploads/Products/" + urunimgname);

                p.resimUrl = "/Uploads/Products/" + urunimgname;
            }

            db.tblUrunler.Add(p);
            db.SaveChanges();
            return RedirectToAction("Urunler");
        }
        [Route("admin/urun/{ad}={id:int}")]
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

        public ActionResult UrunSil(int id)
        {
            var urunsil = db.tblUrunler.Find(id);
            db.tblUrunler.Remove(urunsil);
            db.SaveChanges();
            return RedirectToAction("Urunler");
        }
        [Route("admin/urunduzenle/{ad}={id:int}")]
        public ActionResult UrunDuzenle(int id)
        {
            var urunduzenle = db.tblUrunler.Where(x => x.id == id).SingleOrDefault();
            @ViewBag.kategori = new SelectList(db.tblKategoriler, "id", "ad");
            @ViewBag.stok = new SelectList(db.tblStokYonetimi, "id", "durum");
            @ViewBag.marka = new SelectList(db.tblMarkalar, "id", "ad");
            @ViewBag.cesit = new SelectList(db.tblCesitler, "id", "ad");
            return View(urunduzenle);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult UrunDuzenle(int id, tblUrunler p, HttpPostedFileBase resimUrl)
        {
            if (ModelState.IsValid)
            {
                var urunduzenle = db.tblUrunler.Where(x => x.id == id).SingleOrDefault();
                if (resimUrl != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(urunduzenle.resimUrl)))
                    {
                        System.IO.File.Delete(Server.MapPath(urunduzenle.resimUrl));
                    }

                    WebImage img = new WebImage(resimUrl.InputStream);
                    FileInfo imginfo = new FileInfo(resimUrl.FileName);

                    string urunimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(800, 800);
                    img.Save("~/Uploads/Products/" + urunimgname);

                    p.resimUrl = "/Uploads/Products/" + urunimgname;
                }

                urunduzenle.ad = p.ad;
                urunduzenle.aciklama = p.aciklama;
                urunduzenle.resimUrl = p.resimUrl;
                urunduzenle.fiyat = p.fiyat;
                urunduzenle.stok = p.stok;
                urunduzenle.marka = p.marka;
                urunduzenle.cesit = p.cesit;
                urunduzenle.kategori = p.kategori;
                urunduzenle.teslimat = p.teslimat;
                urunduzenle.renk = p.renk;
                db.SaveChanges();
                return RedirectToAction("Urunler");
            }

            return View(p);
        }

        public ActionResult SliderYoneticisi(int sayfa = 1)
        {
            var slider = db.tblSlider.ToList().OrderByDescending(x => x.id).ToPagedList(sayfa, 3);
            return View(slider);
        }

        public ActionResult SliderEkle()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SliderEkle(tblSlider p, HttpPostedFileBase resimUrl)
        {
            if (resimUrl != null)
            {
                WebImage img = new WebImage(resimUrl.InputStream);
                FileInfo imginfo = new FileInfo(resimUrl.FileName);

                string sliderimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                img.Resize(800, 800);
                img.Save("~/Uploads/Slider/" + sliderimgname);

                p.resimUrl = "/Uploads/Slider/" + sliderimgname;
            }

            db.tblSlider.Add(p);
            db.SaveChanges();
            return RedirectToAction("SliderYoneticisi");
        }

        public ActionResult SliderSil(int id)
        {
            var slidersil = db.tblSlider.Find(id);
            db.tblSlider.Remove(slidersil);
            db.SaveChanges();
            return RedirectToAction("SliderYoneticisi");
        }
        [Route("admin/sliderduzenle/{ad}-{id:int}")]
        public ActionResult SliderDuzenle(int id)
        {
            var sliderduzenle = db.tblSlider.Where(x => x.id == id).SingleOrDefault();
            return View(sliderduzenle);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SliderDuzenle(int id, tblSlider p, HttpPostedFileBase resimUrl)
        {
            if (ModelState.IsValid)
            {
                var sliderduzenle = db.tblSlider.Where(x => x.id == id).SingleOrDefault();
                if (resimUrl != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(sliderduzenle.resimUrl)))
                    {
                        System.IO.File.Delete(Server.MapPath(sliderduzenle.resimUrl));
                    }

                    WebImage img = new WebImage(resimUrl.InputStream);
                    FileInfo imginfo = new FileInfo(resimUrl.FileName);

                    string sliderimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(800, 800);
                    img.Save("~/Uploads/Slider/" + sliderimgname);

                    p.resimUrl = "/Uploads/Slider/" + sliderimgname;
                }

                sliderduzenle.baslik = p.baslik;
                sliderduzenle.resimUrl = p.resimUrl;
                sliderduzenle.aciklama = p.aciklama;
                db.SaveChanges();
                return RedirectToAction("SliderYoneticisi");
            }

            return View(p);
        }

        public ActionResult StokYonetimi(int sayfa=10)
        {
            var stokyonetimi = db.tblStokYonetimi.ToList().ToPagedList(sayfa,10);
            return View(stokyonetimi);
        }

       
    }

}

