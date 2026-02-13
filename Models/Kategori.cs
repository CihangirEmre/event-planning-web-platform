using System.Collections.Generic;

namespace EventPlannerProject.Models
{
    public class Kategori
    {
        public int ID { get; set; }
        public string KategoriAdi { get; set; }

        public ICollection<Etkinlik> Etkinlikler { get; set; }
    }

}
