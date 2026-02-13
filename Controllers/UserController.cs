using EventPlannerProject.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace EventPlannerProject.Controllers
{
    public class UserController : Controller
    {
        private readonly EtkinlikPlatformDBContext _context;

        public UserController(EtkinlikPlatformDBContext context)
        {
            _context = context;
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult JoinEvent(int KullanıcıID, int EtkinlikID)
        {
            // Check if the UserID or EventID is invalid
            if (KullanıcıID == 0 || EtkinlikID == 0)
            {
                TempData["Notification"] = "Invalid data or User not logged in.";
                return RedirectToAction("MainPage", "User");
            }

            try
            {
                // Check if the participant already exists
                var existingParticipant = _context.Katılımcılar
                    .FirstOrDefault(k => k.KullanıcıID == KullanıcıID && k.EtkinlikID == EtkinlikID);

                if (existingParticipant != null)
                {
                    TempData["Notification"] = "You have already joined this event.";
                    return RedirectToAction("MainPage", "User");
                }

                // Fetch the new event details
                var newEvent = _context.Etkinlikler.FirstOrDefault(e => e.ID == EtkinlikID);
                if (newEvent == null)
                {
                    TempData["Notification"] = "Event not found.";
                    return RedirectToAction("MainPage", "User");
                }

                // Get the user's current events
                var userEvents = _context.Katılımcılar
                    .Where(k => k.KullanıcıID == KullanıcıID)
                    .Select(k => k.Etkinlik)
                    .ToList();

                // Check for time conflicts
                foreach (var existingEvent in userEvents)
                {
                    // Calculate existing event's start and end times
                    var existingStart = existingEvent.Tarih.Add(existingEvent.Saat);
                    var existingEnd = existingStart.AddMinutes(existingEvent.EtkinlikSuresi ?? 0);

                    // Calculate new event's start and end times
                    var newEventStart = newEvent.Tarih.Add(newEvent.Saat);
                    var newEventEnd = newEventStart.AddMinutes(newEvent.EtkinlikSuresi ?? 0);

                    // Check if there is a time conflict
                    if ((newEventStart < existingEnd && newEventEnd > existingStart) ||
                        (newEventStart == existingStart || newEventEnd == existingEnd))
                    {
                        TempData["Notification"] = $"This event conflicts with another event you have joined: {existingEvent.EtkinlikAdi}";
                        return RedirectToAction("MainPage", "User");
                    }
                }

                // Add the new participant if no conflicts exist
                var katilimci = new Katilimci
                {
                    KullanıcıID = KullanıcıID,
                    EtkinlikID = EtkinlikID
                };

                _context.Katılımcılar.Add(katilimci);
                _context.SaveChanges();

                TempData["Notification"] = "Joined the event successfully.";
                return RedirectToAction("MainPage", "User");
            }
            catch (Exception ex)
            {
                TempData["Notification"] = "An error occurred while joining the event.";
                return RedirectToAction("MainPage", "User");
            }
        }


        public IActionResult LeaveEvent(int KullanıcıID, int EtkinlikID)
        {
            // Access session data via HttpContext
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (!userId.HasValue || userId.Value == 0)
            {
                TempData["Notification"] = "User not logged in.";
                return RedirectToAction("MainPage", "User");  // Redirect to the page where you want to show the notification
            }

            // Check if the IDs are valid
            if (KullanıcıID == 0 || EtkinlikID == 0)
            {
                TempData["Notification"] = "Invalid data provided.";
                return RedirectToAction("MainPage", "User");  // Redirect to the page where you want to show the notification
            }

            try
            {
                // Check if the user is a participant of the event
                var existingParticipant = _context.Katılımcılar
                    .FirstOrDefault(k => k.KullanıcıID == KullanıcıID && k.EtkinlikID == EtkinlikID);

                if (existingParticipant == null)
                {
                    TempData["Notification"] = "You are not joined to this event.";
                    return RedirectToAction("MainPage", "User");  // Redirect to the page where you want to show the notification
                }

                // Remove the participant from the event
                _context.Katılımcılar.Remove(existingParticipant);
                _context.SaveChanges();

                TempData["Notification"] = "Left the event successfully.";
                return RedirectToAction("MainPage", "User");  // Redirect to the page where you want to show the notification
            }
            catch (Exception ex)
            {
                TempData["Notification"] = "An error occurred while leaving the event.";
                return RedirectToAction("MainPage", "User");  // Redirect to the page where you want to show the notification
            }
        }

        public IActionResult MainPage()
        {
            // Session'dan UserID'yi al
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var kullanici = _context.Kullanıcılar.FirstOrDefault(k => k.ID == userId);
            var allIlgiAlanlari = _context.IlgiAlanlari.ToList();
            var kullaniciIlgiAlaniIds = _context.KullaniciIlgiAlanlari
                .Where(kia => kia.KullanıcıID == userId)
                .Select(kia => kia.IlgiAlanID) 
                .ToList();
            var katilimcilar = _context.Katılımcılar.ToList();

            if (kullanici == null)
            {
                return RedirectToAction("Login", "Account");
            }

            // KullaniciIlgiAlani nesneleri oluşturuluyor
            var kullaniciIlgiAlani = kullaniciIlgiAlaniIds.Select(id => new KullaniciIlgiAlani
            {
                KullanıcıID = userId.Value,
                IlgiAlanID = id,
                Kullanici = kullanici,
                IlgiAlani = allIlgiAlanlari.FirstOrDefault(ilgi => ilgi.ID == id)
            }).ToList();

            // Filter events that match user's interest areas
            var etkinlikler = _context.Etkinlikler
                .Where(e => kullaniciIlgiAlaniIds.Contains((int)e.IlgiAlanID))
                .ToList();
                

            // Puan Hesapla
            int toplamPuan = PuanHesaplama(userId.Value);

            var viewModel = new MainPageViewModel
            {
                Kullanici = kullanici,
                Etkinlikler = etkinlikler,
                KullaniciIlgiAlani = kullaniciIlgiAlani,
                Katilimcilar = katilimcilar,
                ToplamPuan = toplamPuan,
                ProfilFoto = kullanici.ProfilFoto
            };

            return View("~/Views/MainSite/MainPage.cshtml", viewModel);
        }

        public IActionResult Profile()
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null)
            {
                return RedirectToAction("MainPage", "User");
            }

            var kullanici = _context.Kullanıcılar
                    .Include(k => k.Katılımcılar)
                    .ThenInclude(ke => ke.Etkinlik)
                    .Include(k => k.IlgiAlanlari) // Kullanıcıya bağlı ilgi alanları
                    .FirstOrDefault(k => k.ID == userId);

            var allIlgiAlanlari = _context.IlgiAlanlari.ToList();

            // Seçilen ilgi alanlarını belirlemek için SelectListItem oluşturma
            var ilgiAlanlariList = allIlgiAlanlari.Select(ia => new SelectListItem
            {
                Value = ia.ID.ToString(),
                Text = ia.IlgiAlanAdi,
                Selected = kullanici.IlgiAlanlari.Any(kia => kia.IlgiAlanID == ia.ID)
            }).ToList();

            ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList(); // Tüm ilgi alanlarını ViewBag ile gönder
            return View("~/Views/MainSite/Profile.cshtml", kullanici);
        }


        [HttpPost]
        public IActionResult UpdateProfile(string kullaniciAdi, string ad, string soyAd, string email, string telefonNo, int[] selectedIlgiAlanlari)
        {
            int? userId = HttpContext.Session.GetInt32("UserID");
            if (userId == null) { return RedirectToAction("Login", "Account"); }

            var kullanici = _context.Kullanıcılar
                .Include(k => k.IlgiAlanlari)
                .FirstOrDefault(k => k.ID == userId);

            if (kullanici == null) { return RedirectToAction("Login", "Account"); }

            // Kullanıcı bilgilerini güncelle
            kullanici.KullanıcıAdi = kullaniciAdi;
            kullanici.Ad = ad;
            kullanici.Soyad = soyAd;
            kullanici.Email = email;
            kullanici.TelefonNo = telefonNo;

            // Seçili ilgi alanlarını güncelle
            kullanici.IlgiAlanlari.Clear();
            if (selectedIlgiAlanlari != null)
            {
                foreach (var ilgiAlaniId in selectedIlgiAlanlari)
                {
                    kullanici.IlgiAlanlari.Add(new KullaniciIlgiAlani
                    {
                        KullanıcıID = userId.Value,
                        IlgiAlanID = ilgiAlaniId
                    });
                }
            }

            _context.SaveChanges();
            TempData["SuccessMessage"] = "Profil güncellemesi yapılmıştır.";
            return RedirectToAction("MainPage");
        }

        private int PuanHesaplama(int userId)
        {
            // Kullanıcıyı getir
            var kullanici = _context.Kullanıcılar
                .Include(k => k.Katılımcılar)
                .ThenInclude(k => k.Etkinlik)
                .Include(k => k.Puanlar)
                .FirstOrDefault(k => k.ID == userId);

            if (kullanici == null)
            {
                throw new InvalidOperationException("Kullanıcı bulunamadı.");
            }

            int etkinlikKatılımSayisi = kullanici.Katılımcılar.Count;
            int etkinlikOlusturmaSayisi = _context.Etkinlikler.Count(e => e.KullanıcıID == userId);

            bool ilkKatılımBonusAlındı = etkinlikKatılımSayisi > 0;

            int toplamPuan = (etkinlikKatılımSayisi * 10) + (etkinlikOlusturmaSayisi * 15);

            if (ilkKatılımBonusAlındı)
            {
                toplamPuan += 20;
            }

            return toplamPuan;
        }



    }
}

