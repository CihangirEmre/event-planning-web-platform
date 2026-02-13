using System;
using System.Collections.Generic;

namespace EventPlannerProject.Models
{
    public class Etkinlik
    {
        public int ID { get; set; }
        public string EtkinlikAdi { get; set; }
        public string Aciklama { get; set; }
        public DateTime Tarih { get; set; }
        public TimeSpan Saat { get; set; }
        public int? EtkinlikSuresi { get; set; }
        public string? Konum { get; set; }
        public int? KategoriID { get; set; }
        public int? IlgiAlanID { get; set; }
        public int? KullanıcıID { get; set; }
        public bool OnayDurumu { get; set; } = false;
        public Kullanici? Kullanici { get; set; }
        public Kategori? Kategori { get; set; }
        public IlgiAlani? IlgiAlan { get; set; }
        public ICollection<Katilimci>? Katilimcilar { get; set; }
        public ICollection<Puanlar>? Puanlar { get; set; }
        public ICollection<Mesaj>? Mesajlar { get; set; }
        
    }

}
