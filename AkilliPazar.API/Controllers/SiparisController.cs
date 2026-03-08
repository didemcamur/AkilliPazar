using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.DTOs;
using AkilliPazar.Domain.Varliklar;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AkilliPazar.API.Controllers
{
    // Siparis islemleri - Giris gerekli
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SiparisController : ControllerBase
    {
        private readonly ISiparisServisi _siparisServisi;

        public SiparisController(ISiparisServisi siparisServisi)
        {
            _siparisServisi = siparisServisi;
        }

        // Token'dan kullanici Id'sini al
        private string KullaniciId() =>
            User.FindFirstValue(ClaimTypes.NameIdentifier)!;

        // Sepeti siparise donustur
        [HttpPost("olustur")]
        public async Task<IActionResult> SiparisOlustur([FromBody] SiparisOlusturDTO dto)
        {

            try
            {
                var siparis = await _siparisServisi.SiparisOlustur(KullaniciId(), dto);
                return Ok(new
                {
                    Mesaj = "Siparisiniz basariyla olusturuldu.",
                    Siparis = siparis
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Hata = ex.Message });
            }
        }

        // Kullanicinin siparislerini listele
        [HttpGet("siparislerim")]
        public async Task<IActionResult> SiparislerimListele()
        {
            var siparisler = await _siparisServisi.KullaniciSiparisleriniGetir(KullaniciId());
            return Ok(siparisler);
        }

        // Siparis detayi getir
        [HttpGet("{id}")]
        public async Task<IActionResult> SiparisDetay(int id)
        {
            var siparis = await _siparisServisi.SiparisDetayGetir(id, KullaniciId());

            if (siparis == null)
                return NotFound(new { Hata = "Siparis bulunamadi veya bu siparise erisim yetkiniz yok." });

            return Ok(siparis);
        }

        // Tum siparisleri listele - Admin
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/tumsiparisler")]
        public async Task<IActionResult> TumSiparisleriListele()
        {
            var siparisler = await _siparisServisi.TumSiparisleriGetir();
            return Ok(siparisler);
        }

        // Siparis detayi getir - Admin
        [Authorize(Roles = "Admin")]
        [HttpGet("admin/siparis/{id}")]
        public async Task<IActionResult> AdminSiparisDetay(int id)
        {
            var siparis = await _siparisServisi.AdminSiparisDetayGetir(id);

            if (siparis == null)
                return NotFound(new { Hata = "Siparis bulunamadi." });

            return Ok(siparis);
        }

        // Siparis durumunu guncelle - Admin
        [Authorize(Roles = "Admin")]
        [HttpPut("admin/durum-guncelle/{id}")]
        public async Task<IActionResult> SiparisDurumGuncelle(int id, [FromBody] SiparisDurumGuncelleDTO dto)
        {
            var sonuc = await _siparisServisi.SiparisDurumGuncelle(id, dto);

            if (!sonuc)
                return NotFound(new { Hata = "Siparis bulunamadi." });

            return Ok(new
            {
                Mesaj = "Siparis durumu basariyla guncellendi.",
                SiparisId = id,
                YeniDurum = dto.YeniDurum.ToString()
            });
        }

        // Siparis durumlarini listele
        [HttpGet("durumlar")]
        public IActionResult SiparisDurumlariListele()
        {
            var durumlar = Enum.GetValues(typeof(SiparisDurumu))
                .Cast<SiparisDurumu>()
                .Select(d => new
                {
                    Deger = (int)d,
                    Ad = d.ToString()
                })
                .ToList();

            return Ok(durumlar);
        }
    }
}
