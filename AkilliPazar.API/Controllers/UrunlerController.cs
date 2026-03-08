using Microsoft.AspNetCore.Mvc;
using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.DTOs;
using Microsoft.AspNetCore.Authorization;
using AkilliPazar.Domain.Varliklar;
using AutoMapper;
using System.Threading.Tasks;
using System.IO;

namespace AkilliPazar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UrunlerController : ControllerBase
    {
        private readonly IUrunServisi _urunServisi;
        private readonly IMapper _mapper;
        private readonly IWebHostEnvironment _env;

        public UrunlerController(IUrunServisi urunServisi, IMapper mapper, IWebHostEnvironment env)
        {
            _urunServisi = urunServisi;
            _mapper = mapper;
            _env = env;
        }

        // Tum urunleri listele
        [HttpGet]
        public IActionResult TumUrunlerGetir()
        {
            var urunler = _urunServisi.TumUrunleriGetir();
            return Ok(urunler);
        }

        // Id'ye gore urun getir
        [HttpGet("{id:int}")]
        public IActionResult UrunGetir(int id)
        {
            var urun = _urunServisi.IdyeGoreUrunGetir(id);
            if (urun == null)
                return NotFound("Urun bulunamadi");

            return Ok(urun);
        }

        // Yeni urun ekle - Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> UrunEkle([FromForm] UrunEkleDTO dto)
        {
            if (dto.Fiyat <= 0)
                return BadRequest("Fiyat 0'dan buyuk olmalidir");

            if (dto.StokAdedi < 0)
                return BadRequest("Stok adedi negatif olamaz");

            var urun = _mapper.Map<Urun>(dto);

            // Resim yukleme
            if (dto.Resim != null && dto.Resim.Length > 0)
            {
                var resimKlasoru = Path.Combine(_env.WebRootPath ?? "wwwroot", "urun-resimler");
                if (!Directory.Exists(resimKlasoru))
                    Directory.CreateDirectory(resimKlasoru);

                var dosyaAdi = Guid.NewGuid().ToString() + Path.GetExtension(dto.Resim.FileName);
                var kayitYolu = Path.Combine(resimKlasoru, dosyaAdi);

                using (var stream = new FileStream(kayitYolu, FileMode.Create))
                {
                    await dto.Resim.CopyToAsync(stream);
                }

                dto.ResimYolu = $"urun-resimler/{dosyaAdi}";
            }

            _urunServisi.UrunEkle(dto);
            var eklenenUrun = _mapper.Map<Urun>(dto);
            return Ok(new { Mesaj = "Urun eklendi", Urun = eklenenUrun });
        }

        // Urun guncelle - Admin
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public IActionResult UrunGuncelle(int id, [FromBody] UrunGuncelleDTO dto)
        {
            if (dto.Id != id)
                return BadRequest("Id uyusmazligi");

            if (dto.Fiyat <= 0)
                return BadRequest("Fiyat 0'dan buyuk olmalidir");

            if (dto.StokAdedi < 0)
                return BadRequest("Stok adedi negatif olamaz");

            var sonuc = _urunServisi.UrunGuncelle(dto);
            if (!sonuc)
                return NotFound("Urun bulunamadi");

            return Ok("Urun guncellendi");
        }

        // Urun sil - Admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult UrunSil(int id)
        {
            var sonuc = _urunServisi.UrunSil(id);
            if (!sonuc)
                return NotFound("Urun bulunamadi");

            return Ok("Urun silindi");
        }
    }
}
