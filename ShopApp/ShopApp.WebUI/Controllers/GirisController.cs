using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class GirisController : Controller
    {
        // GET: Giris
       ShopAppEntities db=new ShopAppEntities();
        public ActionResult Index()
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(tblMusteriler musteri)
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            var giris = db.tblMusteriler.Where(x => x.email == musteri.email).SingleOrDefault();
            if (giris.email==musteri.email && giris.sifre==musteri.sifre)
            {
                Session["id"] = giris.id;
                Session["email"] = giris.email;
                Session["ad"] = giris.ad;
                Session["soyad"] = giris.soyad;
                Session["sifre"] = giris.sifre;
                Session["adres"] = giris.adres;
                Session["il"] = giris.il;
                Session["ilce"] = giris.ilce;
                Session["resimUrl"] = giris.resimUrl;
                Session["telefon"] = giris.telefon;
                Session["resimUrl"] = giris.resimUrl;
                return RedirectToAction("Index", "Home");
            }
          
            @ViewBag.HataliGiris = "Email Adresiniz ve/veya Şifreniz Hatalı !";
           
            if (giris.email==null)
            {
                HttpNotFound();
            }

            
            return View(musteri);
        }

        public ActionResult Cikis()
        {
            Session["id"] = null;
            Session["email"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Giris");
        }

        public ActionResult Yonetici()
        {
            return View();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Yonetici(tblYonetici p)
        {
            var yonetici = db.tblYonetici.Where(x => x.email == p.email).SingleOrDefault();
            if (yonetici.email==p.email && yonetici.aciklama==p.aciklama)
            {
                Session["yoneticiId"] = yonetici.id;
                Session["yoneticiEmail"] = yonetici.email;
                Session["yoneticiAd"] = yonetici.ad;
                Session["yoneticiSoyad"] = yonetici.soyad;
                Session["yoneticiSifre"] = yonetici.aciklama;
                Session["yoneticiTelefon"] = yonetici.telefon;
                Session["yoneticiResim"] = yonetici.resimUrl;
                return RedirectToAction("Index", "Admin");
            }
            @ViewBag.HataliGiris = "Email Adresiniz ve/veya Şifreniz Hatalı !";
            return View(p);
        }

        public ActionResult YoneticiCikis()
        {
            Session["yoneticiId"] = null;
            Session["yoneticiEmail"] = null;
            Session.Abandon();
            return RedirectToAction("Yonetici", "Giris");

        }
    }
}