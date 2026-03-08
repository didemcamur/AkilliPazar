using AkilliPazar.API.Extensions;
using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AkilliPazar.API.Controllers
{
    // Sepet islemleri - Giris gerekli
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class SepetController : ControllerBase
    {
        private readonly ISepetServisi _sepetServisi;
        private readonly IUrunServisi _urunServisi;

        public SepetController(ISepetServisi sepetServisi, IUrunServisi urunServisi)
        {
            _sepetServisi = sepetServisi;
            _urunServisi = urunServisi;
        }

        // Sepete urun ekle
        [HttpPost("ekle")]
        public IActionResult Ekle([FromBody] SepeteEkleDTO dto)
        {
            var kullaniciId = User.GetUserId();
            if (string.IsNullOrEmpty(kullaniciId))
                return Unauthorized("Kullanici kimlik dogrulamasi basarisiz");

            var urun = _urunServisi.IdyeGoreUrunGetir(dto.UrunId);
            if (urun == null)
                return NotFound("Urun bulunamadi");

            // Stok kontrolu
            if (urun.StokAdedi < dto.Adet)
                return BadRequest($"Yetersiz stok. Mevcut stok: {urun.StokAdedi}");

            if (dto.Adet <= 0)
                return BadRequest("Adet 0'dan buyuk olmalidir");

            _sepetServisi.SepeteEkle(kullaniciId, dto);
            return Ok(new { Mesaj = "Urun sepete eklendi", UrunAdi = urun.Ad, Adet = dto.Adet });
        }

        // Sepeti listele
        [HttpGet("liste")]
        public async Task<IActionResult> Liste()
        {
            var kullaniciId = User.GetUserId();
            if (string.IsNullOrEmpty(kullaniciId))
                return Unauthorized("Kullanici kimlik dogrulamasi basarisiz");

            var sepet = await _sepetServisi.SepetGetir(kullaniciId);
            return Ok(sepet);
        }

        // Sepetten urun kaldir
        [HttpDelete("sil/{urunId:int}")]
        public IActionResult Sil(int urunId)
        {
            var kullaniciId = User.GetUserId();
            if (string.IsNullOrEmpty(kullaniciId))
                return Unauthorized("Kullanici kimlik dogrulamasi basarisiz");

            _sepetServisi.SepettenKaldır(kullaniciId, urunId);
            return Ok("Urun sepetten kaldirildi");
        }

        // Sepet miktar guncelle
        [HttpPut("miktar-guncelle")]
        public IActionResult MiktarGuncelle([FromBody] SepetMiktarGuncelleDTO dto)
        {
            var kullaniciId = User.GetUserId();
            if (string.IsNullOrEmpty(kullaniciId))
                return Unauthorized("Kullanici kimlik dogrulamasi basarisiz");

            var urun = _urunServisi.IdyeGoreUrunGetir(dto.UrunId);
            if (urun == null)
                return NotFound("Urun bulunamadi");

            // Stok kontrolu
            if (urun.StokAdedi < dto.YeniAdet)
                return BadRequest($"Yetersiz stok. Mevcut stok: {urun.StokAdedi}");

            if (dto.YeniAdet <= 0)
                return BadRequest("Adet 0'dan buyuk olmalidir");

            _sepetServisi.SepetMiktarGuncelle(kullaniciId, dto.UrunId, dto.YeniAdet);
            return Ok(new { Mesaj = "Sepet miktari guncellendi", UrunId = dto.UrunId, YeniAdet = dto.YeniAdet });
        }

        // Sepeti temizle
        [HttpDelete("temizle")]
        public IActionResult Temizle()
        {
            var kullaniciId = User.GetUserId();
            if (string.IsNullOrEmpty(kullaniciId))
                return Unauthorized("Kullanici kimlik dogrulamasi basarisiz");

            _sepetServisi.SepetiTemizle(kullaniciId);
            return Ok("Sepet temizlendi");
        }
    }

    // DTO
    public class SepetMiktarGuncelleDTO
    {
        public int UrunId { get; set; }
        public int YeniAdet { get; set; }
    }
}
