using Microsoft.AspNetCore.Mvc;
using EventPlannerProject.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerProject.Controllers
{
    public class EventController : Controller
    {
        private readonly EtkinlikPlatformDBContext _context;

        
        public EventController(EtkinlikPlatformDBContext context)
        {
            _context = context;
        }

        public IActionResult Olustur()
        {
            
            Etkinlik model = new Etkinlik();

            
            ViewData["InterestAreas"] = _context.IlgiAlanlari
                                                 .Select(ia => new SelectListItem
                                                 {
                                                     Value = ia.ID.ToString(),
                                                     Text = ia.IlgiAlanAdi
                                                 })
                                                 .ToList();

            
            ViewData["Categories"] = _context.Kategoriler
                                              .Select(k => new SelectListItem
                                              {
                                                  Value = k.ID.ToString(),
                                                  Text = k.KategoriAdi
                                              })
                                              .ToList();

            return View("~/Views/MainSite/Olustur.cshtml", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Olustur(Etkinlik model)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                TempData["Error"] = "Giriş yapmanız gerekiyor.";
                return RedirectToAction("Login", "Account");
            }

            if (ModelState.IsValid)
            {
                // Etkinlik oluşturan kullanıcıyı ekle
                model.KullanıcıID = userId.Value;

                // Etkinlik kaydet
                _context.Etkinlikler.Add(model);
                _context.SaveChanges();

                // Kullanıcıyı otomatik olarak etkinliğe kat
                var katilimci = new Katilimci
                {
                    KullanıcıID = userId.Value,
                    EtkinlikID = model.ID
                };

                _context.Katılımcılar.Add(katilimci);

                // Puan ekle ve kaydet
               // AddPointsForUser(userId.Value, 15); // Etkinlik oluşturma puanı
                //AddFirstParticipationBonus(userId.Value, model.ID);

                _context.SaveChanges();

                TempData["Success"] = "Etkinlik başarıyla oluşturuldu ve otomatik katılım sağlandı.";
                return RedirectToAction("MainPage", "User");
            }

            // Validasyon hatası durumunda tekrar sayfaya dön
            ViewData["InterestAreas"] = _context.IlgiAlanlari
                                                 .Select(ia => new SelectListItem
                                                 {
                                                     Value = ia.ID.ToString(),
                                                     Text = ia.IlgiAlanAdi
                                                 })
                                                 .ToList();

            ViewData["Categories"] = _context.Kategoriler
                                              .Select(k => new SelectListItem
                                              {
                                                  Value = k.ID.ToString(),
                                                  Text = k.KategoriAdi
                                              })
                                              .ToList();

            return View("~/Views/MainSite/Olustur.cshtml", model);
        }


        public IActionResult Detaylar(int id)
        {
            
            var etkinlik = _context.Etkinlikler
                                   .FirstOrDefault(e => e.ID == id);

            
            if (etkinlik == null)
            {
                return NotFound();
            }

            
            return View("~/Views/MainSite/Detaylar.cshtml", etkinlik);
        }

        public IActionResult Mesajlar(int etkinlikId)
        {
            // Etkinlik ve mesajlar alınır
            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == etkinlikId);
            if (etkinlik == null)
            {
                return NotFound();
            }

            var mesajlar = _context.Mesajlar
                                   .Where(m => m.EtkinlikID == etkinlikId)
                                   .Include(m => m.Gonderici) // Kullanıcı bilgilerini yüklemek için
                                   .OrderBy(m => m.GonderimZamani)
                                   .ToList();

            if (!mesajlar.Any())
            {
                ViewBag.Message = "Bu etkinlik için henüz mesaj bulunmamaktadır.";
            }

            var viewModel = new MesajlarViewModel
            {
                Etkinlik = etkinlik,
                Mesajlar = mesajlar
            };

            return View("~/Views/MainSite/Mesajlar.cshtml", viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult YeniMesaj(int etkinlikId, string mesajMetni)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null || string.IsNullOrEmpty(mesajMetni))
            {
                TempData["Error"] = "Mesaj gönderilemedi.";
                return RedirectToAction("Mesajlar", new { etkinlikId });
            }

            var yeniMesaj = new Mesaj
            {
                GondericiID = userId.Value,
                EtkinlikID = etkinlikId,
                MesajMetni = mesajMetni,
                GonderimZamani = DateTime.Now
            };

            _context.Mesajlar.Add(yeniMesaj);
            _context.SaveChanges();

            return RedirectToAction("Mesajlar", new { etkinlikId });
        }


    }
}
