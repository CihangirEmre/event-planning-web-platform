
namespace EventPlannerProject.Models
{
    public class Katilimci
    {
        public int KullanıcıID { get; set; }
        public int EtkinlikID { get; set; }

        public Kullanici Kullanici { get; set; }
        public Etkinlik Etkinlik { get; set; }
    }

}
