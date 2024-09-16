using eMenu.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace eMenu.Controllers
{
    public class FoodController : Controller
    {
        private eMenuContext db = new eMenuContext();

        // Menüye bağlı yiyecekleri listeleyen action
        public ActionResult ListFoodsByMenu(string menuID)
        {
            // Önce GUID olup olmadığını kontrol edin
            Guid menuGuid;
            if (Guid.TryParse(menuID, out menuGuid))
            {
                // Eğer Guid ise, Guid olarak sorgulayın
                var foods = db.Foods.Where(f => f.menuID == menuGuid).ToList();
                var menu = db.Menus.FirstOrDefault(m => m.menuID == menuGuid);
                if (menu == null)
                {
                    TempData["ErrorMessage"] = "Menü bulunamadı.";
                    return RedirectToAction("Index", "Menu");
                }

                ViewBag.MenuName = menu.name;
                ViewBag.MenuDescription = menu.description;
                return View(foods);
            }
            else
            {
                // Eğer string ise, string olarak sorgulayın
                var foods = db.Foods.Where(f => f.menuID.ToString() == menuID).ToList();
                var menu = db.Menus.FirstOrDefault(m => m.menuID.ToString() == menuID);
                if (menu == null)
                {
                    TempData["ErrorMessage"] = "Menü bulunamadı.";
                    return RedirectToAction("Index", "Menu");
                }

                ViewBag.MenuName = menu.name;
                ViewBag.MenuDescription = menu.description;
                return View(foods);
            }
        }



    }
}
