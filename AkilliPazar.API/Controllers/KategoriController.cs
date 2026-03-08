using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.DTOs;
using AkilliPazar.Domain.Varliklar;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AkilliPazar.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class KategoriController : ControllerBase
    {
        private readonly IKategoriServisi _kategoriServisics;
        private readonly IMapper _mapper;

        public KategoriController(IKategoriServisi kategoriServisi, IMapper mapper)
        {
            _kategoriServisics = kategoriServisi;
            _mapper = mapper;
        }

        // Tum kategorileri listele
        [HttpGet]
        public IActionResult TumKategoriGetir()
        {
            var kategoriler = _kategoriServisics.TumKategorileriGetir();
            return Ok(kategoriler);
        }

        // Id'ye gore kategori getir
        [HttpGet("{id:int}")]
        public IActionResult KategoriGetir(int id)
        {
            var kategori = _kategoriServisics.IdyeGoreKategoriGetir(id);
            if (kategori == null)
                return NotFound("Kategori bulunamadi");

            return Ok(kategori);
        }

        // Yeni kategori ekle - Admin
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult KategoriEkle(KategoriEkleDTO dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Ad))
                return BadRequest("Kategori adi bos olamaz");

            _kategoriServisics.KategoriEkle(dto);
            return Ok("Kategori eklendi");
        }

        // Kategori guncelle - Admin
        [Authorize(Roles = "Admin")]
        [HttpPut("{id:int}")]
        public IActionResult KategoriGuncelle(int id, [FromBody] KategoriGuncelleDTO dto)
        {
            if (dto.Id != id)
                return BadRequest("Id uyusmazligi");

            var mevcutKategori = _kategoriServisics.IdyeGoreKategoriGetir(id);
            if (mevcutKategori == null)
                return NotFound("Kategori bulunamadi");

            _kategoriServisics.KategoriGuncelle(dto);
            return Ok("Kategori guncellendi");
        }

        // Kategori sil - Admin
        [Authorize(Roles = "Admin")]
        [HttpDelete("{id:int}")]
        public IActionResult KategoriSil(int id)
        {
            var kategori = _kategoriServisics.IdyeGoreKategoriGetir(id);
            if (kategori == null)
                return NotFound("Kategori bulunamadi");

            _kategoriServisics.KategoriSil(id);
            return Ok("Kategori silindi");
        }

        // Kategoriye ait urunleri getir
        [HttpGet("{id:int}/urunler")]
        public IActionResult KategoriUrunleriniGetir(int id)
        {
            var urunler = _kategoriServisics.KategoriyeGoreUrunleriGetir(id);

            if (urunler == null || !urunler.Any())
                return NotFound($"ID'si {id} olan kategoriye ait urun bulunamadi.");

            return Ok(urunler);
        }
    }
}
