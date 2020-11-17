using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.UI.WebControls.WebParts;
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
            var toplamsatis = db.tblSiparisKaydi.Count();
            var toplamurun = db.tblUrunler.Count();
            var toplammusteri = db.tblMusteriler.Count();
            var toplamyonetici = db.tblYonetici.Count();
            
            ViewBag.ToplamSatis = toplamsatis;
            ViewBag.ToplamUrun = toplamurun;
            ViewBag.ToplamMusteri = toplammusteri;
            ViewBag.ToplamYonetici = toplamyonetici;
            

            return View();
        }

        public ActionResult SonUrunlerPartial(int sayfa=1)
        {
            return PartialView(db.tblUrunler.ToList().OrderByDescending(x => x.id).ToPagedList(sayfa,5));
        }

        public ActionResult SonSiparislerPartial(int sayfa=1)
        {
            return PartialView(db.tblSiparisKaydi.ToList().OrderByDescending(x => x.id).ToPagedList(sayfa,5));
        }

        public ActionResult ProfilPartial()
        {
            return PartialView(db.tblYonetici.ToList());
        }

        public ActionResult Menuler()
        {
            var menuler = db.tblMenuler.ToList();
            return View(menuler);
        }

        public ActionResult MenuEkle()
        {
            return View();
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult MenuEkle(tblMenuler p)
        {
            db.tblMenuler.Add(p);
            db.SaveChanges();
            return RedirectToAction("Menuler", "Admin");
        }
        [Route("admin/menu/{ad}={id:int}")]
        public ActionResult MenuIncele(int id)
        {
            var menuincele = db.tblMenuler.Find(id);
            return View(menuincele);
        }

        public ActionResult MenuSil(int id)
        {
            var menu = db.tblMenuler.Find(id);
            db.tblMenuler.Remove(menu);
            db.SaveChanges();
            return RedirectToAction("Menuler", "Admin");
        }

        public ActionResult MenuDuzenle(int id)
        {
            var menu = db.tblMenuler.Where(x => x.id == id).SingleOrDefault();
            return View(menu);
        }
        [HttpPost]
        [ValidateInput(false)]
        [ValidateAntiForgeryToken]
        public ActionResult MenuDuzenle(int id, tblMenuler p)
        {
            if (ModelState.IsValid)
            {
                var menu = db.tblMenuler.Where(x => x.id == id).SingleOrDefault();
                menu.baslik = p.baslik;
                menu.metin = p.metin;
                db.SaveChanges();
                return RedirectToAction("Menuler", "Admin");
            }
            return View(p);
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

        public ActionResult Musteriler(int sayfa=1)
        {
            var musteri = db.tblMusteriler.ToList().OrderByDescending(x =>x.id).ToPagedList(sayfa, 10);
            return View(musteri);
        }

        public ActionResult Ayarlar()
        {
            return View(db.tblAyarlar.ToList());
        }

        public ActionResult AyarDuzenle(int id)
        {
            var k = db.tblAyarlar.Where(x => x.id == id).SingleOrDefault();
            return View(k);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult AyarDuzenle(int id, tblAyarlar p, HttpPostedFileBase logourl)
        {
            if (ModelState.IsValid)
            {
                var k = db.tblAyarlar.Where(x => x.id == id).SingleOrDefault();
                if (logourl != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(k.logourl)))
                    {
                        System.IO.File.Delete(Server.MapPath(k.logourl));
                    }

                    WebImage img = new WebImage(logourl.InputStream);
                    FileInfo imginfo = new FileInfo(logourl.FileName);

                    string logoimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(800, 800);
                    img.Save("~/Uploads/Ayarlar/" + logoimgname);

                    p.logourl = "/Uploads/Ayarlar/" + logoimgname;
                }

                k.baslik = p.baslik;
                k.footer = p.footer;
                k.hakkimda = p.hakkimda;
                k.logourl = p.logourl;
                k.title = p.title;
                db.SaveChanges();
                return RedirectToAction("Ayarlar", "Admin");
            }
            return View(p);
        }

        public ActionResult ModulDuzenle1(int id)
        {
            var modul = db.tblAyarlar.Where(x => x.id == id).SingleOrDefault();
            return View(modul);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModulDuzenle1(int id, tblAyarlar p, HttpPostedFileBase modul1)
        {
            if (ModelState.IsValid)
            {
                var modul = db.tblAyarlar.Where(x => x.id == id).SingleOrDefault();
                if (modul1 != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(modul.modul1)))
                    {
                        System.IO.File.Delete(Server.MapPath(modul.modul1));
                    }

                    WebImage img = new WebImage(modul1.InputStream);
                    FileInfo imginfo = new FileInfo(modul1.FileName);

                    string modul1imgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(800, 800);
                    img.Save("~/Uploads/Ayarlar/Modul/" + modul1imgname);

                    p.modul1 = "/Uploads/Ayarlar/Modul/" + modul1imgname;
                }
                modul.modul1 = p.modul1;
                
                db.SaveChanges();
                return RedirectToAction("Ayarlar", "Admin");
            }
            return View(p);
        }
       
        
        public ActionResult ModulDuzenle2(int id)
        {
            var modultwo = db.tblAyarlar.Where(x => x.id == id).SingleOrDefault();
            return View(modultwo);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModulDuzenle2(int id, tblAyarlar p, HttpPostedFileBase modul2)
        {
            if (ModelState.IsValid)
            {
                var modultwo = db.tblAyarlar.Where(x => x.id == id).SingleOrDefault();
                if (modul2 != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(modultwo.modul2)))
                    {
                        System.IO.File.Delete(Server.MapPath(modultwo.modul2));
                    }

                    WebImage img = new WebImage(modul2.InputStream);
                    FileInfo imginfo = new FileInfo(modul2.FileName);

                    string modul2imgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(800, 800);
                    img.Save("~/Uploads/Ayarlar/Modul/" + modul2imgname);

                    p.modul2 = "/Uploads/Ayarlar/Modul/" + modul2imgname;
                }
              
                modultwo.modul2 = p.modul2;
              
                db.SaveChanges();
                return RedirectToAction("Ayarlar", "Admin");
            }
            return View(p);
        }

       
        public ActionResult ModulDuzenle3(int id)
        {
            var modulthree = db.tblAyarlar.Where(x => x.id == id).SingleOrDefault();
            return View(modulthree);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ModulDuzenle3(int id, tblAyarlar p, HttpPostedFileBase modul3)
        {
            if (ModelState.IsValid)
            {
                var modulthree = db.tblAyarlar.Where(x => x.id == id).SingleOrDefault();
                if (modul3 != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(modulthree.modul3)))
                    {
                        System.IO.File.Delete(Server.MapPath(modulthree.modul3));
                    }

                    WebImage img = new WebImage(modul3.InputStream);
                    FileInfo imginfo = new FileInfo(modul3.FileName);

                    string modul3imgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(800, 800);
                    img.Save("~/Uploads/Ayarlar/Modul/" + modul3imgname);

                    p.modul3 = "/Uploads/Ayarlar/Modul/" + modul3imgname;
                }
                modulthree.modul3 = p.modul3;
                db.SaveChanges();
                return RedirectToAction("Ayarlar", "Admin");
            }
            return View(p);
        }



    }

}

