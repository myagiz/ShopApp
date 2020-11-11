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
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Index(tblMusteriler musteri)
        {
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
                return RedirectToAction("Index", "Profil");
            }

            @ViewBag.HataliGiris = "Email Adresiniz ve/veya Şifreniz Hatalı !";
            return View(musteri);
        }

        public ActionResult Cikis()
        {
            Session["id"] = null;
            Session["email"] = null;
            Session.Abandon();
            return RedirectToAction("Index", "Giris");
        }
    }
}