using System;
using System.Collections.Generic;
using EventPlannerProject.Models;


namespace EventPlannerProject.Models
{
    public class Kullanici
    {
        public int ID { get; set; }
        public string KullanıcıAdi { get; set; }
        public string Sifre { get; set; }
        public string Email { get; set; }
        public string? Konum { get; set; }
        public string Ad { get; set; }
        public string Soyad { get; set; }
        public DateTime? DogumTarihi { get; set; }
        public string Cinsiyet { get; set; }
        public string TelefonNo { get; set; }
        public byte[]? ProfilFoto { get; set; }
        public string Salt { get; set; }
        public bool IsAdmin { get; set; }

        public ICollection<Katilimci> Katılımcılar { get; set; }
        public ICollection<Mesaj> GonderilenMesajlar { get; set; }
        //public ICollection<Mesaj> AlinanMesajlar { get; set; }
        public ICollection<Puanlar> Puanlar { get; set; }
        public ICollection<KullaniciIlgiAlani> IlgiAlanlari { get; set; }
        public ICollection<Etkinlik> Etkinlikler { get; set; }

    }

}
