using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using eMenu.Models;
using eMenu.Models.MenuModels;
using eMenu.Security.Session;

namespace eMenu.Controllers
{

    [RoleAuthorize(1)]
    public class PanelController : Controller
    {
        // GET: Panel

        [HttpGet]
        public ActionResult MenuEkle()
        {
            return View();
        }

        [HttpPost]
        public ActionResult MenuEkle(Menus menus, HttpPostedFileBase imagePath)
        {
            using (var context = new eMenuContext())
            {
                try
                {
                    // Kategorinin benzersiz olup olmadığını kontrol etmek için var mı diye bakın
                    var menu = context.Menus.FirstOrDefault(k => k.name == menus.name);
                    if (menu != null)
                    {
                        TempData["ErrorMessage"] = "Bu menü zaten kullanılmaktadır.";
                        return View();
                    }

                    // menuID için yeni bir Guid oluştur
                    menus.menuID = Guid.NewGuid();

                    // Eğer resim dosyası yüklendi ise
                    if (imagePath != null && imagePath.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(imagePath.FileName);
                        var kaydetmeYolu = Path.Combine(Server.MapPath("/template/src/assets/images/menus/"), fileName);
                        imagePath.SaveAs(kaydetmeYolu);
                        menus.imagePath = "/template/src/assets/images/menus/" + fileName;
                    }

                    // Menüyü veritabanına ekle
                    context.Menus.Add(menus);
                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Menü başarıyla eklendi.";
                    return View();
                }
                catch (DbEntityValidationException ex)
                {
                    var errorMessages = new List<string>();
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            errorMessages.Add($"Property: {validationError.PropertyName}, Error: {validationError.ErrorMessage}");
                        }
                    }

                    TempData["ErrorMessage"] = "Menü eklenirken bir hata oluştu: " + string.Join("; ", errorMessages);
                    return View();
                }
            }
        }

        [HttpGet]
        public ActionResult MenuSil()
        {
            using (var context = new eMenuContext())
            {
                var menus = context.Menus.ToList();
                ViewBag.MenusList = new SelectList(menus, "menuID", "name");
                return View();
            }
        }

        [HttpPost]
        public ActionResult MenuSil(Guid menuID)
        {
            using (var context = new eMenuContext())
            {
                // Kategoriyi bul
                var menu = context.Menus.FirstOrDefault(k => k.menuID == menuID);
                if (menu == null)
                {
                    TempData["ErrorMessage"] = "Menü bulunamadı.";
                    return View();
                }

                try
                {
                    // Kategoriye bağlı alt kategorileri bul
                    var Foods = context.Foods.Where(ak => ak.menuID == menuID).ToList();

                    // Alt kategorilere bağlı ürünleri bul ve sil
                    foreach (var food in Foods)
                    {
                        var foods = context.Foods.Where(u => u.foodsID == food.foodsID).ToList();
                        context.Foods.RemoveRange(foods); // Alt kategoriye bağlı ürünleri sil
                    }

                    // Alt kategorileri sil
                    context.Foods.RemoveRange(Foods);

                    // Kategoriye bağlı ürünleri sil
                    var FoodsInMenu = context.Foods.Where(u => u.menuID == menuID).ToList();
                    context.Foods.RemoveRange(FoodsInMenu);

                    // Kategoriyi sil
                    context.Menus.Remove(menu);

                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Menü ve bağlı yiyecek/içecekler başarıyla silindi.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = "Menü silinirken bir hata oluştu: " + ex.Message;
                }

                var menus = context.Menus.ToList();
                ViewBag.MenusList = new SelectList(menus, "menuID", "name");

                return View();
            }
        }


        eMenuContext context = new eMenuContext();
        MenuDuzenleViewModel menuDuzenle = new MenuDuzenleViewModel();

        [HttpGet]
        public ActionResult MenuDuzenle()
        {
            menuDuzenle.Menus = new SelectList(context.Menus, "menuID", "name");


            return View(menuDuzenle);
        }

        [HttpPost]
        public ActionResult MenuDuzenle(Menus menus, HttpPostedFileBase imagePath)
        {
            try
            {
                // Kategoriyi bul
                var menu = context.Menus.FirstOrDefault(k => k.menuID == menus.menuID);
                if (menu == null)
                {
                    TempData["ErrorMessage"] = "Menü bulunamadı.";
                    return View();
                }

                // Kategorinin benzersiz olup olmadığını kontrol etmek için var mı diye bakın
                var menu2 = context.Menus.FirstOrDefault(k => k.name == menus.name && k.menuID != menus.menuID);
                if (menu2 != null)
                {
                    TempData["ErrorMessage"] = "Bu menü zaten kullanılmaktadır.";
                    return View();
                }

                // Eğer resim dosyası yüklendi ise
                if (imagePath != null && imagePath.ContentLength > 0)
                {
                    // Dosya adını al
                    var fileName = Path.GetFileName(imagePath.FileName);
                    // Dosyayı kaydetmek istediğiniz klasörü belirleyin
                    var kaydetmeYolu = Path.Combine(Server.MapPath("/template/src/assets/images/menus/"), fileName);
                    // Dosyayı kopyala
                    imagePath.SaveAs(kaydetmeYolu);
                    // Veritabanına kaydedilecek olan resim yolu
                    menus.imagePath = "/template/src/assets/images/menus/" + fileName;
                }

                // Kategoriyi güncelle
                menu.name = menus.name;
                menu.imagePath = menus.imagePath;
                menu.description = menus.description;

                context.SaveChanges();

                TempData["SuccessMessage"] = "Menü başarıyla güncellendi.";
            }
            catch (DbEntityValidationException ex)
            {
                foreach (var validationErrors in ex.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                    }
                }
                // Güncelleme sırasında bir hata oluşursa, hata mesajını göster
                TempData["ErrorMessage"] = "Menü güncellenirken bir hata oluştu.";
            }
            catch (Exception ex)
            {
                // Güncelleme sırasında bir hata oluşursa, hata mesajını göster
                TempData["ErrorMessage"] = "Menü güncellenirken bir hata oluştu: " + ex.Message;
            }

            // ViewModel'i oluşturarak View'e dön
            var menuDuzenle = new MenuDuzenleViewModel();
            menuDuzenle.Menus = new SelectList(context.Menus, "menuID", "name");

            return View(menuDuzenle);
        }



        [HttpGet]
        public ActionResult YiyecekEkle()
        {
            var MenuDuzenleViewModel = new MenuDuzenleViewModel
            {
                Menus = new SelectList(context.Menus, "menuID", "name"),
                Foods = new SelectList(context.Foods, "foodsID", "name")
            };
            return View(MenuDuzenleViewModel);
        }

        [HttpPost]
        public ActionResult YiyecekEkle(Foods foods,HttpPostedFileBase imagePath)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    if (imagePath != null && imagePath.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(imagePath.FileName);
                        var kaydetmeYolu = Path.Combine(Server.MapPath("/template/src/assets/images/foods/"), fileName);
                        imagePath.SaveAs(kaydetmeYolu);
                        foods.imagePath = "/template/src/assets/images/foods/" + fileName;
                    }

                    foods.foodsID = Guid.NewGuid();

                    context.Foods.Add(foods);
                    context.SaveChanges();
                    TempData["SuccessMessage"] = "Yiyecek başarıyla eklendi.";
                    return RedirectToAction("YiyecekEkle");
                }
                catch (DbEntityValidationException ex)
                {
                    // Doğrulama hatası oluştuğunda her bir hatayı al ve hata mesajını ModelState'e ekle
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                            TempData["ErrorMessage"] = "Yiyecek eklenemedi.";
                        }
                    }
                }
            }

            // Doğrulama başarısız olduğunda, aynı görünümü tekrar göster
            var MenuDuzenleViewModel = new MenuDuzenleViewModel
            {
                Menus = new SelectList(context.Menus, "menuID", "name"),
                Foods = new SelectList(context.Foods, "foodsID", "name")
            };
            return View(MenuDuzenleViewModel);
        }

        [HttpGet]
        public ActionResult YiyecekSil()
        {
            var MenuDuzenleViewModel = new MenuDuzenleViewModel
            {
                Menus = new SelectList(context.Menus, "menuID", "name"),
                Foods = new SelectList(context.Foods, "foodsID", "name")
            };
            return View(MenuDuzenleViewModel);
        }

        public JsonResult GetFoods(Guid menuID)
        {

            var altKategoriler = (from u in context.Foods
                                  where u.menuID == menuID
                                  select new
                                  {
                                      Text = u.name,
                                      Value = u.foodsID.ToString()
                                  }).ToList();
            return Json(altKategoriler, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult YiyecekSil(Guid foodsID)
        {
            eMenuContext context = new eMenuContext();
            MenuDuzenleViewModel menuDuzenle = new MenuDuzenleViewModel();

            try
            {
                // Silinecek ürünü bul
                var food = context.Foods.FirstOrDefault(u => u.foodsID == foodsID);

                // Ürünü bulamazsa hata mesajı göster ve Sil view'ine geri dön
                if (food == null)
                {
                    TempData["ErrorMessage"] = "Yiyecek/İçecek bulunamadı.";
                    return View();
                }

                // Ürünü veritabanından sil
                context.Foods.Remove(food);
                context.SaveChanges();

                // Başarı mesajı göster ve Sil view'ine geri dön
                TempData["SuccessMessage"] = "Yiyecek/İçecek başarıyla silindi.";

                // Kategorileri çek
                var menus = context.Menus.ToList();

                // Alt kategorileri çek
                var foods = context.Foods.ToList();

                menuDuzenle.Menus = new SelectList(menus, "menuID", "name");
                menuDuzenle.Foods = new SelectList(foods, "foodsID", "name");
                return View(menuDuzenle);
            }
            catch (DbUpdateException ex)
            {
                var innerException = ex.InnerException;
                while (innerException != null)
                {
                    ViewBag.ErrorMessage += innerException.Message + "<br/>";
                    innerException = innerException.InnerException;
                }
                return View();
            }
        }


        [HttpGet]
        public ActionResult YiyecekDuzenle()
        {

            eMenuContext context = new eMenuContext();
            MenuDuzenleViewModel menuDuzenle = new MenuDuzenleViewModel();

            menuDuzenle.Menus = new SelectList(context.Menus, "menuID", "name");
            menuDuzenle.Foods = new SelectList(context.Foods, "foodsID", "name");

            return View(menuDuzenle);
        }

        [HttpPost]
        public ActionResult YiyecekDuzenle(Foods foods, HttpPostedFileBase imagePath)
        {
            if (foods == null)
            {
                // Eğer altKategoriler null ise, yeni bir nesne oluştur
                foods = new Foods();
                ModelState.AddModelError("", "Yiyecek/İçecek bilgileri alınamadı. Lütfen tekrar deneyin.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    eMenuContext context = new eMenuContext();

                    // Alt kategoriyi bul
                    var food = context.Foods.FirstOrDefault(a => a.foodsID == foods.foodsID);
                    if (food == null)
                    {
                        TempData["ErrorMessage"] = "Alt kategori bulunamadı.";
                        return View();
                    }

                    // Eğer resim dosyası yüklendi ise
                    if (imagePath != null && imagePath.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(imagePath.FileName);
                        var kaydetmeYolu = Path.Combine(Server.MapPath("/template/src/assets/images/foods/"), fileName);
                        imagePath.SaveAs(kaydetmeYolu);
                        food.imagePath = "/template/src/assets/images/foods/" + fileName;
                    }

                    // Diğer alt kategori özelliklerini güncelle
                    food.menuID = foods.menuID;
                    food.name = foods.name;
                    food.price = foods.price;
                    food.description = foods.description;
                    // Değişiklikleri kaydet
                    context.SaveChanges();

                    TempData["SuccessMessage"] = "Yiyecek/İçecek başarıyla güncellendi.";
                }
                catch (Exception ex)
                {
                    // Güncelleme sırasında bir hata oluşursa, hata mesajını göster
                    ViewBag.ErrorMessage = "Yiyecek/İçecek güncellenirken bir hata oluştu: " + ex.Message;
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Geçersiz model verisi.";
            }

            // Doğrulama başarısız olduğunda veya geçersiz model verisi olduğunda, aynı görünümü tekrar göster
            eMenuContext newContext = new eMenuContext();
            MenuDuzenleViewModel menuDuzenle = new MenuDuzenleViewModel();
            menuDuzenle.Menus = new SelectList(newContext.Menus, "menuID", "name");
            menuDuzenle.Foods = new SelectList(newContext.Foods, "foodsID", "name");

            return View(menuDuzenle);
        }

    }
}