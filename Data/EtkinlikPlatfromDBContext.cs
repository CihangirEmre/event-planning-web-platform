using Microsoft.EntityFrameworkCore;
using EventPlannerProject.Models;


public class EtkinlikPlatformDBContext : DbContext
{
    public EtkinlikPlatformDBContext(DbContextOptions<EtkinlikPlatformDBContext> options) : base(options) { }

    public DbSet<Kullanici> Kullanıcılar { get; set; }
    public DbSet<Etkinlik> Etkinlikler { get; set; }
    public DbSet<Kategori> Kategoriler { get; set; }
    public DbSet<IlgiAlani> IlgiAlanlari { get; set; }
    public DbSet<Mesaj> Mesajlar { get; set; }
    public DbSet<Katilimci> Katılımcılar { get; set; }
    public DbSet<Puanlar> Puanlar { get; set; }
    public DbSet<KullaniciIlgiAlani> KullaniciIlgiAlanlari { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Katilimci ilişkisi
        modelBuilder.Entity<Katilimci>()
            .HasKey(k => new { k.KullanıcıID, k.EtkinlikID });

        modelBuilder.Entity<Katilimci>()
            .HasOne(k => k.Kullanici)
            .WithMany(k => k.Katılımcılar)
            .HasForeignKey(k => k.KullanıcıID);

        modelBuilder.Entity<Katilimci>()
            .HasOne(k => k.Etkinlik)
            .WithMany(e => e.Katilimcilar)
            .HasForeignKey(k => k.EtkinlikID);

        // Mesaj ilişkisi
        modelBuilder.Entity<Mesaj>()
    .HasOne(m => m.Gonderici)
    .WithMany(k => k.GonderilenMesajlar)
    .HasForeignKey(m => m.GondericiID)
    .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Mesaj>()
            .HasOne(m => m.Etkinlik)
            .WithMany(e => e.Mesajlar)
            .HasForeignKey(m => m.EtkinlikID)
            .OnDelete(DeleteBehavior.Cascade);

        // KullaniciIlgiAlani ilişkisi
        modelBuilder.Entity<KullaniciIlgiAlani>()
            .HasKey(k => new { k.KullanıcıID, k.IlgiAlanID });

        modelBuilder.Entity<KullaniciIlgiAlani>()
            .HasOne(ka => ka.Kullanici)
            .WithMany(k => k.IlgiAlanlari)
            .HasForeignKey(ka => ka.KullanıcıID);

        modelBuilder.Entity<KullaniciIlgiAlani>()
            .HasOne(ka => ka.IlgiAlani)
            .WithMany(i => i.KullaniciIlgiAlanlari)
            .HasForeignKey(ka => ka.IlgiAlanID);

        // Puan ilişkisi
        modelBuilder.Entity<Puanlar>()
            .HasKey(p => new { p.KullanıcıID, p.KazanilanTarih });

        modelBuilder.Entity<Puanlar>()
            .HasOne(p => p.Kullanici)
            .WithMany(k => k.Puanlar)
            .HasForeignKey(p => p.KullanıcıID);

        modelBuilder.Entity<Puanlar>()
            .HasOne(p => p.Etkinlik)
            .WithMany(e => e.Puanlar)
            .HasForeignKey(p => p.EtkinlikID)
            .OnDelete(DeleteBehavior.SetNull);

        modelBuilder.Entity<Etkinlik>()
    .HasOne(e => e.Kullanici)
    .WithMany(k => k.Etkinlikler)
    .HasForeignKey(e => e.KullanıcıID)
    .OnDelete(DeleteBehavior.SetNull);
        // Etkinlik ve İlgi Alanı Arasındaki İlişki
        modelBuilder.Entity<Etkinlik>()
            .HasOne(e => e.IlgiAlan)
            .WithMany(i => i.Etkinlikler) // İlgi Alanı birden çok etkinlik ile ilişkilendirilebilir
            .HasForeignKey(e => e.IlgiAlanID) // Foreign key
            .OnDelete(DeleteBehavior.Restrict); // İlişki silme davranışı

    }
}
