using AkilliPazar.Application.Servisler;
using AkilliPazar.Domain.Varliklar;
using AkilliPazar.Infrastructure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace AkilliPazar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TokenServisi _tokenServisi;
        private readonly RoleManager<IdentityRole> _roleManager;

        public AuthController(UserManager<ApplicationUser> userManager, TokenServisi tokenServisi, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _tokenServisi = tokenServisi;
            _roleManager = roleManager;
        }

        // Yeni kullanici kaydi
        [HttpPost("register")]
        public async Task<IActionResult> Register(string email, string sifre, string adSoyad)
        {
            var mevcutKullanici = await _userManager.FindByEmailAsync(email);
            if (mevcutKullanici != null)
                return BadRequest("Bu email adresi zaten kayitli!");

            var yeniKullanici = new ApplicationUser
            {
                UserName = email,
                Email = email,
                AdSoyad = adSoyad
            };

            var sonuc = await _userManager.CreateAsync(yeniKullanici, sifre);

            if (!sonuc.Succeeded)
                return BadRequest(sonuc.Errors);

            // Varsayilan User rolu ata
            if (await _roleManager.RoleExistsAsync("User"))
                await _userManager.AddToRoleAsync(yeniKullanici, "User");

            return Ok("Kayit islemi basarili!");
        }

        // Kullanici girisi
        [HttpPost("login")]
        public async Task<IActionResult> Login(string email, string sifre)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return Unauthorized("Kullanici bulunamadi!");

            var sifreDogruMu = await _userManager.CheckPasswordAsync(user, sifre);
            if (!sifreDogruMu)
                return Unauthorized("Sifre yanlis!");

            var roller = await _userManager.GetRolesAsync(user);
            var token = _tokenServisi.TokenUret(user, roller);

            return Ok(new
            {
                Token = token,
                Roller = roller,
                KullaniciAdi = user.AdSoyad,
                Email = user.Email
            });
        }

        // Yeni rol olustur - Admin
        [Authorize(Roles = "Admin")]
        [HttpPost("rol-olustur")]
        public async Task<ActionResult> RolOlustur(string rolAdi)
        {
            if (await _roleManager.RoleExistsAsync(rolAdi))
                return BadRequest("Bu rol zaten var");

            var result = await _roleManager.CreateAsync(new IdentityRole(rolAdi));
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"{rolAdi} rolu olusturuldu");
        }

        // Kullaniciya rol ata - Admin
        [Authorize(Roles = "Admin")]
        [HttpPost("kullaniciya-rol-ekle")]
        public async Task<ActionResult> KullaniciyaRolEkle(string email, string rolAdi)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Kullanici Bulunamadi.");

            if (!await _roleManager.RoleExistsAsync(rolAdi))
                return BadRequest("Rol bulunamadi. Once rol olusturun.");

            var result = await _userManager.AddToRoleAsync(user, rolAdi);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"'{email}' kullanicisina '{rolAdi}' rolu eklendi");
        }

        // Kullanicidan rol kaldir - Admin
        [Authorize(Roles = "Admin")]
        [HttpPost("kullanicidan-rol-kaldir")]
        public async Task<ActionResult> KullanicidanRolKaldir(string email, string rolAdi)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest("Kullanici Bulunamadi.");

            var result = await _userManager.RemoveFromRoleAsync(user, rolAdi);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            return Ok($"'{email}' kullanicisindan '{rolAdi}' rolu kaldirildi");
        }

        // Mevcut kullanici bilgilerini getir
        [Authorize]
        [HttpGet("profil")]
        public async Task<IActionResult> Profil()
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Kullanici bulunamadi");

            var roller = await _userManager.GetRolesAsync(user);

            return Ok(new
            {
                Id = user.Id,
                Email = user.Email,
                AdSoyad = user.AdSoyad,
                Roller = roller
            });
        }

        // Profil guncelle
        [Authorize]
        [HttpPut("profil-guncelle")]
        public async Task<IActionResult> ProfilGuncelle([FromBody] ProfilGuncelleDTO dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Kullanici bulunamadi");

            // AdSoyad guncelle
            if (!string.IsNullOrWhiteSpace(dto.AdSoyad))
                user.AdSoyad = dto.AdSoyad;

            // Email guncelle (varsa)
            if (!string.IsNullOrWhiteSpace(dto.Email) && dto.Email != user.Email)
            {
                var emailKullaniliyorMu = await _userManager.FindByEmailAsync(dto.Email);
                if (emailKullaniliyorMu != null && emailKullaniliyorMu.Id != user.Id)
                    return BadRequest("Bu email adresi zaten kullaniliyor!");

                user.Email = dto.Email;
                user.UserName = dto.Email;
            }

            var sonuc = await _userManager.UpdateAsync(user);
            if (!sonuc.Succeeded)
                return BadRequest(sonuc.Errors);

            return Ok(new { Mesaj = "Profil basariyla guncellendi" });
        }

        // Sifremi unuttum - Reset token olustur
        [HttpPost("sifremi-unuttum")]
        public async Task<IActionResult> SifremiUnuttum([FromBody] SifremiUnuttumDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                // Guvenlik icin kullanici bulunamasa bile basarili mesaj don
                return Ok(new { Mesaj = "Eger bu email kayitli ise, sifre sifirlama linki email adresinize gonderildi." });
            }

            // Reset token olustur
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            
            // Not: Gercek uygulamada burada email gonderilir
            // Simdilik token'i response'da donuyoruz (sadece development icin)
            // Production'da token'i email ile gondermelisiniz

            return Ok(new 
            { 
                Mesaj = "Sifre sifirlama token'i olusturuldu. (Development modu - token response'da)",
                Token = token // Production'da bu satiri kaldirin
            });
        }

        // Sifre sifirla
        [HttpPost("sifre-sifirla")]
        public async Task<IActionResult> SifreSifirla([FromBody] SifreSifirlaDTO dto)
        {
            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
                return BadRequest("Kullanici bulunamadi");

            var sonuc = await _userManager.ResetPasswordAsync(user, dto.Token, dto.YeniSifre);
            if (!sonuc.Succeeded)
                return BadRequest(sonuc.Errors);

            return Ok(new { Mesaj = "Sifreniz basariyla sifirlandi" });
        }

        // Sifre degistir (giris yapmis kullanici icin)
        [Authorize]
        [HttpPost("sifre-degistir")]
        public async Task<IActionResult> SifreDegistir([FromBody] SifreDegistirDTO dto)
        {
            var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
            if (string.IsNullOrEmpty(userId))
                return Unauthorized();

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound("Kullanici bulunamadi");

            // Eski sifreyi kontrol et
            var sifreDogruMu = await _userManager.CheckPasswordAsync(user, dto.EskiSifre);
            if (!sifreDogruMu)
                return BadRequest("Eski sifre yanlis");

            // Yeni sifreyi guncelle
            var sonuc = await _userManager.ChangePasswordAsync(user, dto.EskiSifre, dto.YeniSifre);
            if (!sonuc.Succeeded)
                return BadRequest(sonuc.Errors);

            return Ok(new { Mesaj = "Sifreniz basariyla degistirildi" });
        }
    }

    // DTO'lar
    public class ProfilGuncelleDTO
    {
        public string? AdSoyad { get; set; }
        public string? Email { get; set; }
    }

    public class SifremiUnuttumDTO
    {
        public string Email { get; set; } = null!;
    }

    public class SifreSifirlaDTO
    {
        public string Email { get; set; } = null!;
        public string Token { get; set; } = null!;
        public string YeniSifre { get; set; } = null!;
    }

    public class SifreDegistirDTO
    {
        public string EskiSifre { get; set; } = null!;
        public string YeniSifre { get; set; } = null!;
    }
}
