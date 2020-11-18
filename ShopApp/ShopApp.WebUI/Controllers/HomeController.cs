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
using ShopApp.WebUI.Models.Home;

namespace ShopApp.WebUI.Controllers
{
    public class HomeController : Controller
    {
        // GET: Home
        ShopAppEntities db = new ShopAppEntities();

        [Route("")]
        [Route("home")]
        public ActionResult Index(int sayfa = 1)
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            var urunler = db.tblUrunler.ToList().OrderByDescending(x => x.id).ToPagedList(sayfa, 8);
            return View(urunler);
        }

        public ActionResult MenuPartial()
        {
            return PartialView(db.tblMenuler.ToList().OrderBy(x => x.id));
        }

        [Route("home/{ad}-{id:int}")]
        public ActionResult Menu(int id)
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            if (id == null)
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
            @ViewBag.Kategoriler = db.tblKategoriler.ToList().OrderBy(x => x.ad);
            return PartialView();
        }

        [Route("home/kategori/{ad}-{id:int}")]
        public ActionResult Kategori(int id, int sayfa = 1)
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            var kategori = db.tblUrunler.Include("tblKategoriler").OrderByDescending(x => x.id)
                .Where(x => x.tblKategoriler.id == id).ToList()
                .ToPagedList(sayfa, 12);
            return View(kategori);
        }

        //public ActionResult SiparisKaydi(int id)
        //{
        //    ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
        //    if (id == null)
        //    {
        //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        //    }

        //    tblSiparisKaydi p = db.tblSiparisKaydi.Find(id);
        //    if (p == null)
        //    {
        //        return HttpNotFound();
        //    }

        //    return View(p);
        //}

        public ActionResult KayitOl()
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult KayitOl(tblMusteriler p, HttpPostedFileBase resimUrl)
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
            return RedirectToAction("Index", "Giris");
        }

        public ActionResult Profil(int id)
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

        //public ActionResult Sepet()
        //{
          
        //    ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
        //    return View();
        //}

        public ActionResult Checkout()
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            return View();
        }
        public ActionResult CheckoutDetails()
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            return View();
        }
        public ActionResult DecreaseQty(int productId)
        {
            if (Session["cart"] != null)
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var product = db.tblUrunler.Find(productId);
                foreach (var item in cart)
                {
                    if (item.Product.id == productId)
                    {
                        int prevQty = item.Quantity;
                        if (prevQty > 0)
                        {
                            cart.Remove(item);
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = prevQty - 1
                            });
                        }
                        break;
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect("Checkout");
        }
        public ActionResult AddToCart(int productId, string url)
        {
            if (Session["cart"] == null)
            {
                List<Item> cart = new List<Item>();
                var product = db.tblUrunler.Find(productId);
                cart.Add(new Item()
                {
                    Product = product,
                    Quantity = 1
                });
                Session["cart"] = cart;
            }
            else
            {
                List<Item> cart = (List<Item>)Session["cart"];
                var count = cart.Count();
                var product = db.tblUrunler.Find(productId);
                for (int i = 0; i < count; i++)
                {
                    if (cart[i].Product.id == productId)
                    {
                        int prevQty = cart[i].Quantity;
                        cart.Remove(cart[i]);
                        cart.Add(new Item()
                        {
                            Product = product,
                            Quantity = prevQty + 1
                        });
                        break;
                    }
                    else
                    {
                        var prd = cart.Where(x => x.Product.id == productId).SingleOrDefault();
                        if (prd == null)
                        {
                            cart.Add(new Item()
                            {
                                Product = product,
                                Quantity = 1
                            });
                        }
                    }
                }
                Session["cart"] = cart;
            }
            return Redirect(url);
        }
        public ActionResult RemoveFromCart(int productId)
        {
            List<Item> cart = (List<Item>)Session["cart"];
            foreach (var item in cart)
            {
                if (item.Product.id == productId)
                {
                    cart.Remove(item);
                    break;
                }
            }
            Session["cart"] = cart;
            return RedirectToAction("Checkout", "Home");
        }

        public ActionResult SiparisOnay()
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            ViewBag.musteri = new SelectList(db.tblMusteriler, "id", "ad & soyad");
            return View();
        }
        [HttpPost]
        public ActionResult SiparisOnay(tblSiparisKaydi p)
        {
            var onay = db.tblSiparisKaydi.Add(p);
            db.SaveChanges();
            return RedirectToAction("Index", "Home");
        }


    }
}