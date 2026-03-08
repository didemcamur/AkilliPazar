using AkilliPazar.Domain.Varliklar;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AkilliPazar.Infrastructure.VeriTabani
{
    public class SmartMarketDbContext : IdentityDbContext<ApplicationUser>
    {
        public SmartMarketDbContext(DbContextOptions<SmartMarketDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Kategoriler Primary Key tanimlama
            modelBuilder.Entity<Kategoriler>()
                .HasKey(k => k.Id);

            // Urun Primary Key tanimlama
            modelBuilder.Entity<Urun>()
                .HasKey(u => u.Id);

            // Urun-Kategori iliskisi
            modelBuilder.Entity<Urun>()
                .HasOne(u => u.Kategori)
                .WithMany(k => k.Urunler)
                .HasForeignKey(u => u.KategoriId)
                .OnDelete(DeleteBehavior.Cascade);

            // Siparis-Kullanici iliskisi
            modelBuilder.Entity<Siparis>()
                .HasOne(s => s.Kullanici)
                .WithMany()
                .HasForeignKey(s => s.KullaniciId)
                .OnDelete(DeleteBehavior.Cascade);

            // SiparisUrun-Siparis iliskisi
            modelBuilder.Entity<SiparisUrun>()
                .HasOne(su => su.Siparis)
                .WithMany(s => s.SiparisUrunleri)
                .HasForeignKey(su => su.SiparisId)
                .OnDelete(DeleteBehavior.Cascade);

            // SiparisUrun-Urun iliskisi
            modelBuilder.Entity<SiparisUrun>()
                .HasOne(su => su.Urun)
                .WithMany()
                .HasForeignKey(su => su.UrunId)
                .OnDelete(DeleteBehavior.Restrict); // Urun silindiginde siparis kayitlari korunur

            // Tablo isimleri
            modelBuilder.Entity<Kategoriler>().ToTable("Kategoriler");
            modelBuilder.Entity<Urun>().ToTable("Urunler");
            modelBuilder.Entity<Siparis>().ToTable("Siparisler");
            modelBuilder.Entity<SiparisUrun>().ToTable("SiparisUrunleri");
        }

        // Tablo tanimlari
        public DbSet<Urun> Urunler { get; set; }
        public DbSet<Kategoriler> Kategoriler { get; set; }
        public DbSet<Sepet> Sepetler { get; set; }
        public DbSet<SepetUrun> SepetUrunleri { get; set; }

        // Siparis tablolari
        public DbSet<Siparis> Siparisler { get; set; }
        public DbSet<SiparisUrun> SiparisUrunleri { get; set; }
    }
}
