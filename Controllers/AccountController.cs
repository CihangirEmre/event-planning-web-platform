using Microsoft.AspNetCore.Mvc;
using EventPlannerProject.Models;
using System.Security.Cryptography;
using System.Text;
using System.Linq;

namespace EventPlannerProject.Controllers
{
    public class AccountController : Controller
    {
        private readonly EtkinlikPlatformDBContext _context;

        public AccountController(EtkinlikPlatformDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View("~/Views/UserActions/UserLogin.cshtml");
        }

        [HttpPost]
        public IActionResult LoginPost(string username, string password)
        {
            // Kullanıcı adı ile kullanıcıyı bul
            var kullanici = _context.Kullanıcılar.FirstOrDefault(k => k.KullanıcıAdi == username);
            if (kullanici == null)
            {
                ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre";
                return View("~/Views/UserActions/UserLogin.cshtml");
            }



            // Kullanıcının salt değerini al ve girdiği şifreyi hashle
            string hashedPassword = HashPassword(password, kullanici.Salt);
            if (hashedPassword == kullanici.Sifre)
            {
                HttpContext.Session.SetInt32("UserID", kullanici.ID);

                if (kullanici.IsAdmin == true)
                {
                    // Admin ise AdminPanel sayfasına yönlendiricek
                    return RedirectToAction("AdminPage", "Admin");
                }
                else
                {
                    return RedirectToAction("MainPage", "User");
                }
            }
            else
            {
                ViewBag.ErrorMessage = "Geçersiz kullanıcı adı veya şifre";
                return View("~/Views/UserActions/UserLogin.cshtml");
            }
        }

        [HttpGet]
        public IActionResult Register()
        {
            ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
            return View("~/Views/UserActions/Register.cshtml");
        }


        [HttpPost]
        public IActionResult Register(string username, string email, string password, string confirmPassword, string firstName, string lastName, DateTime birthDate, string gender, string phoneNumber, List<int> selectedInterests, IFormFile profilePhoto, string Location)
        {
            if (password != confirmPassword)
            {
                ViewBag.ErrorMessage = "Şifreler eşleşmiyor";
                ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                return View("~/Views/UserActions/Register.cshtml");
            }

            if (!ValidatePassword(password))
            {
                ViewBag.ErrorMessage = "Şifre en az 8 karakter uzunluğunda, bir büyük harf, bir küçük harf ve bir sayı içermelidir.";
                ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                return View("~/Views/UserActions/Register.cshtml");
            }

            var existingUser = _context.Kullanıcılar.FirstOrDefault(k => k.KullanıcıAdi == username || k.Email == email);
            if (existingUser != null)
            {
                ViewBag.ErrorMessage = "Bu kullanıcı adı veya e-posta zaten alınmış.";
                ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                return View("~/Views/UserActions/Register.cshtml");
            }

            // Profil fotoğrafını kontrol et ve byte dizisine çevir veritabanına ekle
            byte[] photoBytes = null;

            if (profilePhoto != null && profilePhoto.Length > 0)
            {
                //JPG formatında mı kontrol et
                if (profilePhoto.ContentType != "image/jpeg")
                {
                    ViewBag.ErrorMessage = "Profil fotoğrafı sadece JPG formatında olmalıdır.";
                    ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                    return View("~/Views/UserActions/Register.cshtml");
                }

                // Maksimum boyut 2MB kontrol et
                if (profilePhoto.Length > 2 * 1024 * 1024)
                {
                    ViewBag.ErrorMessage = "Profil fotoğrafı 2MB'den büyük olamaz.";
                    ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                    return View("~/Views/UserActions/Register.cshtml");
                }
                //byte dizisine çevir(daha az alan kaplıyo)
                using (var memoryStream = new MemoryStream())
                {
                    profilePhoto.CopyTo(memoryStream);
                    photoBytes = memoryStream.ToArray();
                }
            }

            // Yeni kullanıcı için salt ve hash oluşturma
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(password, salt);

            var newUser = new Kullanici
            {
                KullanıcıAdi = username,
                Email = email,
                Sifre = hashedPassword, 
                Salt = salt, 
                Ad = firstName,
                Soyad = lastName,
                DogumTarihi = birthDate,
                Cinsiyet = gender,
                TelefonNo = phoneNumber,
                ProfilFoto = photoBytes,
                Konum = Location
            };

            _context.Kullanıcılar.Add(newUser);
            _context.SaveChanges();

            // Seçilen ilgi alanlarını veritabanına ekle
            if (selectedInterests != null && selectedInterests.Any())
            {
                foreach (var interestId in selectedInterests)
                {
                    var kullaniciIlgiAlani = new KullaniciIlgiAlani
                    {
                        KullanıcıID = newUser.ID,
                        IlgiAlanID = interestId
                    };
                    _context.KullaniciIlgiAlanlari.Add(kullaniciIlgiAlani);
                }
                _context.SaveChanges();
            }

            TempData["SuccessMessage"] = "Profil bilgilerinizi burdan güncelleyebilirsiniz\nUnutmayın tek amacımız var eğlence";
            return RedirectToAction("Login");
        }



        [HttpGet]
        public IActionResult ForgotPassword()
        {
            return View("~/Views/UserActions/ForgotPassword.cshtml");
        }

        [HttpPost]
        public IActionResult ResetPassword(string username, string newPassword, string confirmPassword)
        {
            if (newPassword != confirmPassword)
            {
                ViewBag.ErrorMessage = "Şifreler eşleşmiyor";
                ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                return View("~/Views/UserActions/ForgotPassword.cshtml");
            }

            if (!ValidatePassword(newPassword))
            {
                ViewBag.ErrorMessage = "Şifre en az 5 karakter uzunluğunda, bir büyük harf, bir küçük harf ve bir sayı içermelidir.";
                ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                return View("~/Views/UserActions/ForgotPassword.cshtml");
            }

            var kullanici = _context.Kullanıcılar.FirstOrDefault(k => k.KullanıcıAdi == username);
            if (kullanici == null)
            {
                ViewBag.ErrorMessage = "Kullanıcı bulunamadı.";
                ViewBag.IlgiAlanlari = _context.IlgiAlanlari.ToList();
                return View("~/Views/UserActions/ForgotPassword.cshtml");
            }

            // Şifreyi hashleyerek güncelleme
            string salt = GenerateSalt();
            string hashedPassword = HashPassword(newPassword, salt);
            kullanici.Sifre = hashedPassword;
            kullanici.Salt = salt;
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Şifreniz başarıyla güncellenmiştir.";
            return RedirectToAction("Login");
        }

        private bool ValidatePassword(string password)
        {
            return password.Length >= 8
                   && password.Any(char.IsUpper)
                   && password.Any(char.IsLower)
                   && password.Any(char.IsDigit);
        }

        // Salt oluşturma fonksiyonu
        private string GenerateSalt()
        {
            byte[] saltBytes = new byte[16];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(saltBytes);
            }
            return Convert.ToBase64String(saltBytes);
        }

        // Şifreyi hashleme fonksiyonu (SHA256)
        private string HashPassword(string password, string salt)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] saltedPassword = Encoding.UTF8.GetBytes(password + salt);
                byte[] hashBytes = sha256.ComputeHash(saltedPassword);
                return Convert.ToBase64String(hashBytes);
            }
        }
    }
}
