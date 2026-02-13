using System.Collections.Generic;

namespace EventPlannerProject.Models
{
    public class IlgiAlani
    {
        public int ID { get; set; }
        public string IlgiAlanAdi { get; set; }

        public ICollection<KullaniciIlgiAlani> KullaniciIlgiAlanlari { get; set; }
        public ICollection<Etkinlik> Etkinlikler { get; set; }
    }

}
