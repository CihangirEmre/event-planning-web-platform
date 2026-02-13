using System;

namespace EventPlannerProject.Models
{
    public class Mesaj
    {
        public int MesajID { get; set; }
        public int GondericiID { get; set; }
        public string MesajMetni { get; set; }
        public DateTime GonderimZamani { get; set; }
        public int EtkinlikID { get; set; }
        public Kullanici Gonderici { get; set; }
        public Etkinlik Etkinlik { get; set; }
    }

}
