using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using eMenu.Models;

namespace eMenu.Controllers
{
    public class AuthController : Controller
    {
        // GET: Auth

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            using (var context = new eMenuContext())
            {
                var result = context.User.Where(u => u.email == email && u.password == password).SingleOrDefault();
                if (result == null)
                {
                    TempData["ErrorMessage"] = "E-mail veya şifreniz hatalı!";
                    return View();
                }
                else
                {
                    Session["user"] = result;
                    TempData["SuccessMessage"] = "Hoş geldiniz " + result.name + " " + result.surname;
                    return RedirectToAction("Index", "Menu");

                }
            }
        }

        public ActionResult Logout()
        {
            // Session'ı temizle
            Session.Clear();
            Session.Abandon();

            // Çıkış yapıldı mesajını ayarla
            TempData["SuccessMessage"] = "Hoşça kalın 👋";

            // TempData'nın bir sonraki istek boyunca saklanmasını sağla
            TempData.Keep("SuccessMessage");

            // Login sayfasına yönlendir ve mesajı gönder
            return View("Login");
        }


        // GET: Register
        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            using (var context = new eMenuContext())
            {
                // E-posta adresinin benzersiz olup olmadığını kontrol etmek için var mı diye bakın
                var existingUser = context.User.FirstOrDefault(k => k.email == user.email);
                if (existingUser != null)
                {
                    TempData["ErrorMessage"] = "Bu e-posta adresi zaten kullanılmaktadır.";
                    return View();
                }

                // Kullanici Rol Atama
                user.authorityID = 2;

                // Kullanıcıyı veritabanına ekleyin
                context.User.Add(user);
                context.SaveChanges();
                TempData["SuccessMessage"] = "Hesabınız başarıyla oluşturuldu.";

                return RedirectToAction("Login", "Auth");
            }
        }


    }

}