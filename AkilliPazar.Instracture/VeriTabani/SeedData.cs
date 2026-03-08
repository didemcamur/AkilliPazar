using AkilliPazar.Domain.Varliklar;
using AkilliPazar.Infrastructure;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AkilliPazar.Infrastructure.VeriTabani
{
    // Ornek verileri olusturan sinif
    public static class SeedData
    {
        // Ana metod - tum seed verilerini olusturur
        public static async Task SeedVerileriniOlustur(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<SmartMarketDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await context.Database.MigrateAsync();
            await RolleriOlustur(roleManager);
            await KullanicilariOlustur(userManager);
            await KategorileriOlustur(context);
            await UrunleriOlustur(context);
        }

        // Rolleri olustur
        private static async Task RolleriOlustur(RoleManager<IdentityRole> roleManager)
        {
            string[] roller = { "Admin", "User" };

            foreach (var rol in roller)
            {
                if (!await roleManager.RoleExistsAsync(rol))
                {
                    await roleManager.CreateAsync(new IdentityRole(rol));
                    Console.WriteLine($"[SEED] '{rol}' rolu olusturuldu.");
                }
            }
        }

        // Ornek kullanicilari olustur
        private static async Task KullanicilariOlustur(UserManager<ApplicationUser> userManager)
        {
            // Admin kullanici
            var adminEmail = "admin@akillipazar.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    AdSoyad = "Admin Kullanici",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                    Console.WriteLine($"[SEED] Admin olusturuldu: {adminEmail} / Admin123!");
                }
            }

            // Normal kullanici
            var userEmail = "kullanici@akillipazar.com";
            if (await userManager.FindByEmailAsync(userEmail) == null)
            {
                var user = new ApplicationUser
                {
                    UserName = userEmail,
                    Email = userEmail,
                    AdSoyad = "Ahmet Yilmaz",
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, "User123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, "User");
                    Console.WriteLine($"[SEED] Kullanici olusturuldu: {userEmail} / User123!");
                }
            }
        }

        // Ornek kategorileri olustur
        private static async Task KategorileriOlustur(SmartMarketDbContext context)
        {
            if (await context.Kategoriler.AnyAsync())
                return;

            var kategoriler = new List<Kategoriler>
            {
                new Kategoriler { Ad = "Elektronik" },
                new Kategoriler { Ad = "Giyim" },
                new Kategoriler { Ad = "Ev ve Yasam" },
                new Kategoriler { Ad = "Spor" },
                new Kategoriler { Ad = "Kitap" }
            };

            await context.Kategoriler.AddRangeAsync(kategoriler);
            await context.SaveChangesAsync();
            Console.WriteLine($"[SEED] {kategoriler.Count} kategori olusturuldu.");
        }

        // Ornek urunleri olustur
        private static async Task UrunleriOlustur(SmartMarketDbContext context)
        {
            if (await context.Urunler.AnyAsync())
                return;

            var kategoriler = await context.Kategoriler.ToListAsync();
            if (!kategoriler.Any())
                return;

            var elektronikId = kategoriler.FirstOrDefault(k => k.Ad == "Elektronik")?.Id ?? 1;
            var giyimId = kategoriler.FirstOrDefault(k => k.Ad == "Giyim")?.Id ?? 2;
            var sporId = kategoriler.FirstOrDefault(k => k.Ad == "Spor")?.Id ?? 4;
            var kitapId = kategoriler.FirstOrDefault(k => k.Ad == "Kitap")?.Id ?? 5;

            var urunler = new List<Urun>
            {
                new Urun { Ad = "iPhone 15 Pro", Aciklama = "Apple iPhone 15 Pro 256GB", Fiyat = 64999.99m, StokAdedi = 25, KategoriId = elektronikId },
                new Urun { Ad = "Samsung Galaxy S24", Aciklama = "Samsung Galaxy S24 Ultra 512GB", Fiyat = 54999.99m, StokAdedi = 30, KategoriId = elektronikId },
                new Urun { Ad = "MacBook Pro M3", Aciklama = "Apple MacBook Pro 14 inc", Fiyat = 89999.99m, StokAdedi = 15, KategoriId = elektronikId },
                new Urun { Ad = "AirPods Pro 2", Aciklama = "Apple AirPods Pro 2. Nesil", Fiyat = 7999.99m, StokAdedi = 50, KategoriId = elektronikId },
                new Urun { Ad = "Erkek Gomlek", Aciklama = "Pamuklu slim fit beyaz gomlek", Fiyat = 599.99m, StokAdedi = 100, KategoriId = giyimId },
                new Urun { Ad = "Kot Pantolon", Aciklama = "Mavi slim fit kot pantolon", Fiyat = 749.99m, StokAdedi = 80, KategoriId = giyimId },
                new Urun { Ad = "Kosu Ayakkabisi", Aciklama = "Nike Air Zoom Pegasus", Fiyat = 3499.99m, StokAdedi = 40, KategoriId = sporId },
                new Urun { Ad = "Yoga Mati", Aciklama = "5mm kalinlik yoga mati", Fiyat = 299.99m, StokAdedi = 60, KategoriId = sporId },
                new Urun { Ad = "Clean Code", Aciklama = "Robert C. Martin", Fiyat = 199.99m, StokAdedi = 100, KategoriId = kitapId },
                new Urun { Ad = "Suc ve Ceza", Aciklama = "Dostoyevski", Fiyat = 89.99m, StokAdedi = 150, KategoriId = kitapId }
            };

            await context.Urunler.AddRangeAsync(urunler);
            await context.SaveChangesAsync();
            Console.WriteLine($"[SEED] {urunler.Count} urun olusturuldu.");
        }
    }
}
