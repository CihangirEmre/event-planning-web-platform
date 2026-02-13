namespace EventPlannerProject.Models
{
    public class AdminSiteViewModel
    {
        public Kullanici Kullanici { get; set; } // Kullanıcı Detayları için
        
        public IEnumerable<Etkinlik> Etkinlikler { get; set; } // Etkinlik Yönetimi için
        public IEnumerable<IlgiAlani> IlgiAlanlari { get; set; } // İlgi Alanları Yönetimi için
    }
}
