using System;
using EventPlannerProject.Models;

namespace EventPlannerProject.Models
{
public class Puanlar
{
    public int KullanıcıID { get; set; }
    public int Puan { get; set; }
    public DateTime KazanilanTarih { get; set; }
    public int? EtkinlikID { get; set; }

    public Kullanici Kullanici { get; set; }
    public Etkinlik Etkinlik { get; set; }
}
}

