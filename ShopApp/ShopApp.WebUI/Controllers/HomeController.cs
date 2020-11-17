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
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            var urunler = db.tblUrunler.ToList().OrderByDescending(x =>x.id).ToPagedList(sayfa, 8);
            return View(urunler);
        }
       
        public ActionResult MenuPartial()
        {
            return PartialView(db.tblMenuler.ToList().OrderBy(x =>x.id));
        }
        [Route("home/{ad}-{id:int}")]
        public ActionResult Menu(int id)
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            if (id== null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            tblMenuler p = db.tblMenuler.Find(id);
            if (p == null)
            {
                return HttpNotFound();
            }

            return View(p);

        }

        public ActionResult UrunKategoriPartial()
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            return PartialView(db.tblKategoriler.Include("tblUrunler").ToList().OrderBy(x => x.ad));
        }
        [Route("home/urunincele/{ad}-{id:int}")]
        public ActionResult UrunIncele(int id)
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
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
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            var kategori = db.tblUrunler.Include("tblKategoriler").OrderByDescending(x =>x.id).Where(x => x.tblKategoriler.id == id).ToList()
                .ToPagedList(sayfa, 12);
            return View(kategori);
        }

        public ActionResult SiparisKaydi(int id)
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
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
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
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
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
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

        public ActionResult Sepet()
        {
            var urunler = db.tblUrunler.ToList();
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            return View(urunler);
        }

        public ActionResult DecreaseQty(int urunId)
        {
            if (Session["sepet"] != null)
            {
                List<tblSiparisKaydi> sepet = (List<tblSiparisKaydi>)Session["sepet"];
                var product = db.tblUrunler.Find(urunId);
                foreach (var item in sepet)
                {
                    if (item.tblUrunler.id == urunId)
                    {
                        var prevQty = item.miktar;
                        if (prevQty > 0)
                        {
                            sepet.Remove(item);
                            sepet.Add(new tblSiparisKaydi()
                            {
                                tblUrunler = product,
                                miktar = prevQty - 1
                            });
                        }
                        break;
                    }
                }
                Session["sepet"] = sepet;
            }
            return Redirect("Checkout");
        }

        public ActionResult SepeteEkle(int urunId, string url)
        {
            if (Session["sepet"]==null)
            {
                List<tblSiparisKaydi>sepet=new List<tblSiparisKaydi>();
                var urun = db.tblUrunler.Find(urunId);
                sepet.Add(new tblSiparisKaydi()
                {
                    tblUrunler = urun,
                    miktar=1
                   
                });
                Session["sepet"] = sepet;
            }
            else
            {
                List<tblSiparisKaydi> sepet = (List <tblSiparisKaydi>) Session["sepet"];
                var count = sepet.Count;
                var urun = db.tblUrunler.Find(urunId);
                for (int i = 0; i < count; i++)
                {
                    if (sepet[i].tblUrunler.id==urunId)
                    {
                        var prevQty = sepet[i].miktar;
                        sepet.Remove(sepet[i]);
                        sepet.Add(new tblSiparisKaydi()
                        {
                            tblUrunler = urun,
                            miktar=prevQty+1
                        });
                        break;
                    }
                    else
                    {
                        var p = sepet.Where(x => x.tblUrunler.id == urunId).SingleOrDefault();
                        if (p==null)
                        {
                            sepet.Add(new tblSiparisKaydi()
                            {
                                tblUrunler = urun,
                                miktar = 1
                            });
                        }
                    }
                }

                Session["sepet"] = sepet;
            }

            return Redirect(url);
        }

        public ActionResult SepetSil(int urunId)
        {
            List<tblSiparisKaydi> sepet = (List<tblSiparisKaydi>)Session["sepet"];
            foreach (var item in sepet)
            {
                if (item.tblUrunler.id== urunId)
                {
                    sepet.Remove(item);
                    break;
                }
            }
            Session["sepet"] = sepet;
            return Redirect("Index");
        }
        
    }
}