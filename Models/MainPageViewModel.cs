using System.Collections.Generic;
using EventPlannerProject.Models;

namespace EventPlannerProject.Models
{
    public class MainPageViewModel
    {
        public Kullanici Kullanici { get; set; }
        public int ToplamPuan { get; set; }
        public byte[] ProfilFoto { get; set; }
        public IEnumerable<Etkinlik> Etkinlikler { get; set; }
        public IEnumerable<IlgiAlani> AllIlgiAlanlari { get; set; }
        public IEnumerable<KullaniciIlgiAlani> KullaniciIlgiAlani { get; set; }
        public IEnumerable<Katilimci> Katilimcilar { get; set; }
    }
}
