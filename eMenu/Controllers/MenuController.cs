using eMenu.Models;  // Models klasöründeki Menus sınıfına erişmek için
using System;
using System.Linq;
using System.Web.Mvc;

namespace eMenu.Controllers
{
    public class MenuController : Controller
    {
        private eMenuContext db = new eMenuContext();  // Veri tabanı bağlantısı

        public ActionResult Index()
        {
            // Menus tablosundaki tüm kayıtları getiriyoruz
            var menus = db.Menus.ToList();

            Console.WriteLine(menus.Count);

            return View(menus);  // Menuleri Index.cshtml'e gönderiyoruz
        }
    }
}
