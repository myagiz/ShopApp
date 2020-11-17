using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using ShopApp.WebUI.Models;

namespace ShopApp.WebUI.Controllers
{
    public class ProfilController : Controller
    {
        // GET: Profil
       ShopAppEntities db=new ShopAppEntities();
        public ActionResult Index()
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            var profil = db.tblMusteriler.ToList();
            return View(profil);
        }
        [HttpGet]
        public ActionResult Duzenle(int id)
        {
            ViewBag.Ayarlar = db.tblAyarlar.SingleOrDefault();
            var profil = db.tblMusteriler.Where(x => x.id == id).SingleOrDefault();
            return View(profil);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Duzenle(int id,tblMusteriler musteri,HttpPostedFileBase resimUrl)
        {
            if (ModelState.IsValid)
            {
                var profil = db.tblMusteriler.Where(x => x.id == id).SingleOrDefault();
                if (resimUrl != null)
                {
                    if (System.IO.File.Exists(Server.MapPath(profil.resimUrl)))
                    {
                        System.IO.File.Delete(Server.MapPath(profil.resimUrl));
                    }

                    WebImage img = new WebImage(resimUrl.InputStream);
                    FileInfo imginfo = new FileInfo(resimUrl.FileName);

                    string profilimgname = Guid.NewGuid().ToString() + imginfo.Extension;
                    img.Resize(300, 300);
                    img.Save("~/Uploads/Profil/" + profilimgname);

                    profil.resimUrl = "/Uploads/Profil/" + profilimgname;
                }

                profil.ad = musteri.ad;
                profil.soyad = musteri.soyad;
                profil.email = musteri.email;
                profil.telefon = musteri.telefon;
                profil.sifre = musteri.sifre;
                profil.adres = musteri.adres;
                profil.il = musteri.il;
                profil.ilce = musteri.ilce;
                profil.sipariskaydi = musteri.sipariskaydi;
                db.SaveChanges();
                Session["id"] = null;
                Session["email"] = null;
                Session.Abandon();
                return RedirectToAction("Index", "Giris");
            }
            return View(musteri);
        }
    }
}