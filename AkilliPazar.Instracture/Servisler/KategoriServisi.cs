using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.DTOs;
using AkilliPazar.Domain.Varliklar;
//using AkilliPazar.Infrastructure.Migrations;
using AkilliPazar.Infrastructure.VeriTabani;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Infrastructure.Servisler
{
    public class KategoriServisi:IKategoriServisi
    {
        private readonly SmartMarketDbContext _context;
        private readonly IMapper _mapper;


        public KategoriServisi(SmartMarketDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public IEnumerable<Kategoriler> TumKategorileriGetir()
        {
            return _context.Kategoriler.ToList();
        }
        public Kategoriler? IdyeGoreKategoriGetir(int id)
        {
            var kategori=_context.Kategoriler.FirstOrDefault(x => x.Id == id);
            return kategori;
        }

        public void KategoriEkle(KategoriEkleDTO dto)
        {
            var Yeni_kategori = _mapper.Map<Kategoriler>(dto);
            _context.Kategoriler.Add(Yeni_kategori);
            _context.SaveChanges();
        }

        public void KategoriGuncelle(KategoriGuncelleDTO dto)
        {
            var mevcutkategori = _context.Kategoriler.Find(dto.Id);
            if (mevcutkategori != null)
            {
                // Güncelleme işleminde map uygulanıyor
                _mapper.Map(dto, mevcutkategori);
                _context.SaveChanges();
            }
        }

        public void KategoriSil(int id)
        {
            var kategori=_context.Kategoriler.FirstOrDefault(i => i.Id == id);
            if(kategori != null)
            {
                _context.Kategoriler.Remove(kategori);
                _context.SaveChanges();
            }
        }
        //=> Servis interface`i üzerindeki metodu implemente ediyoruz.
        public IEnumerable<Urun> KategoriyeGoreUrunleriGetir(int kategoriId)
        {
            var kategori = _context.Kategoriler
                                   .Include(k => k.Urunler)
                                   .FirstOrDefault(k => k.Id == kategoriId);
            if (kategori == null)
                return Enumerable.Empty<Urun>();

            return kategori.Urunler;
        }

    }
}
