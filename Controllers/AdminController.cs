using Microsoft.AspNetCore.Mvc;
using EventPlannerProject.Models;
using Microsoft.EntityFrameworkCore;

namespace EventPlannerProject.Controllers
{
    public class AdminController : Controller
    {
        private readonly EtkinlikPlatformDBContext _context;

        public AdminController(EtkinlikPlatformDBContext context)
        {
            _context = context;
        }

        // Admin ana sayfası
        public IActionResult AdminPage()
        {
            return View("~/Views/AdminSite/AdminPage.cshtml");
        }

        // Kullanıcı Yönetimi
        public IActionResult KullanıcıYönetimi()
        {
            try
            {
                var kullanıcılar = _context.Kullanıcılar
                    .Select(k => new { k.ID, k.KullanıcıAdi, k.Ad, k.Soyad })
                    .ToList();

                if (!kullanıcılar.Any())
                {
                    ViewBag.Message = "Hiç kullanıcı bulunamadı.";
                }

                return View("~/Views/AdminSite/KullanıcıYönetimi.cshtml", kullanıcılar);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
                return View("~/Views/AdminSite/KullanıcıYönetimi.cshtml", new List<object>());
            }
        }

        // Kullanıcı Silme
        [HttpPost]
        public IActionResult SilKullanıcı(int id)
        {
            var kullanıcı = _context.Kullanıcılar.FirstOrDefault(k => k.ID == id);
            if (kullanıcı != null)
            {
                _context.Kullanıcılar.Remove(kullanıcı);
                _context.SaveChanges();
            }
            return RedirectToAction("KullanıcıYönetimi");
        }

        // Kullanıcı Detayları
        public IActionResult KullanıcıDetayı(int id)
        {
            var kullanıcı = _context.Kullanıcılar
                .Include(k => k.IlgiAlanlari)
                .ThenInclude(ka => ka.IlgiAlani)
                .FirstOrDefault(k => k.ID == id);

            if (kullanıcı == null)
            {
                return RedirectToAction("KullanıcıYönetimi");
            }

            var viewModel = new AdminSiteViewModel
            {
                Kullanici = kullanıcı,
                IlgiAlanlari = kullanıcı.IlgiAlanlari.Select(ka => ka.IlgiAlani)
            };

            return View("~/Views/AdminSite/KullanıcıDetayı.cshtml", viewModel);
        }

        // Etkinlik Yönetimi
        public IActionResult EtkinlikYönetimi()
        {
            try
            {
                var etkinlikler = _context.Etkinlikler
      .Include(e => e.Kategori) // Kategori dahil
      .Include(e => e.IlgiAlan) // İlgi Alanı dahil
      .Select(e => new
      {
          e.ID,
          e.EtkinlikAdi,
          e.Tarih,
          e.Saat,
          Kategori = e.Kategori.KategoriAdi,
          IlgiAlani = e.IlgiAlan.IlgiAlanAdi // İlgi Alanı Adı
      })
      .ToList();

                if (!etkinlikler.Any())
                {
                    ViewBag.Message = "Hiç etkinlik bulunamadı.";
                }

                return View("~/Views/AdminSite/EtkinlikYönetimi.cshtml", etkinlikler);
            }
            catch (Exception ex)
            {
                ViewBag.ErrorMessage = $"Bir hata oluştu: {ex.Message}";
                return View("~/Views/AdminSite/EtkinlikYönetimi.cshtml", new List<object>());
            }
        }

        // Etkinlik Onaylama
        [HttpPost]
        public IActionResult OnaylaEtkinlik(int id)
        {
            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id);
            if (etkinlik != null)
            {
                etkinlik.OnayDurumu = true; // Onay durumu için bir sütun
                _context.SaveChanges();
            }
            return RedirectToAction("EtkinlikYönetimi");
        }

        // Etkinlik Silme
        [HttpPost]
        public IActionResult SilEtkinlik(int id)
        {
            var etkinlik = _context.Etkinlikler.FirstOrDefault(e => e.ID == id);
            if (etkinlik != null)
            {
                _context.Etkinlikler.Remove(etkinlik);
                _context.SaveChanges();
            }
            return RedirectToAction("EtkinlikYönetimi");
        }
    }
}
