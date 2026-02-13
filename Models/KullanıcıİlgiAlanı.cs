namespace EventPlannerProject.Models
{
    public class KullaniciIlgiAlani
    {
        public int KullanıcıID { get; set; }
        public int IlgiAlanID { get; set; }

        public Kullanici Kullanici { get; set; }
        public IlgiAlani IlgiAlani { get; set; }
    }

}

