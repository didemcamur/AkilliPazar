using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.DTOs;
using AkilliPazar.Domain.Varliklar;
using AkilliPazar.Infrastructure.VeriTabani;
using System.Collections.Generic;
using System.Linq;
using AutoMapper;
using Microsoft.EntityFrameworkCore;

namespace AkilliPazar.Infrastructure.Servisler
{
    public class UrunServisi : IUrunServisi
    {
        private readonly SmartMarketDbContext _context;
        private readonly IMapper _mapper;

       

        public UrunServisi(SmartMarketDbContext context,IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }
        //Tüm ürünleri getir
        public IEnumerable<Urun> TumUrunleriGetir()
        {
            return _context.Urunler
                .Include(u => u.Kategori)
                .ToList();
        }
        // Id'ye göre ürün getir
        public Urun? IdyeGoreUrunGetir(int id)
        {
            return _context.Urunler
                .Include(u => u.Kategori)
                .FirstOrDefault(u => u.Id == id);
        }
        //Ürün Ekle 
        public void UrunEkle(UrunEkleDTO urunEkleDto)
        {
            var Yeni_urun = _mapper.Map<Urun>(urunEkleDto);
            _context.Urunler.Add(Yeni_urun);
            _context.SaveChanges();
        }
        //Ürün Güncelle
        public bool UrunGuncelle(UrunGuncelleDTO urunGuncelleDto)
        {
            var mevcutUrun = _context.Urunler.Find(urunGuncelleDto.Id);
            if (mevcutUrun != null)
            {
                // Güncelleme işleminde map uygulanıyor
                _mapper.Map(urunGuncelleDto, mevcutUrun);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
        public bool UrunSil (int id)
        {
            var urun = _context.Urunler.Find(id);
            if (urun != null)
            {
                _context.Urunler.Remove(urun);
                _context.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
