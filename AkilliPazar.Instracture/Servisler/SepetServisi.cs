using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.DTOs;
using AkilliPazar.Domain.Varliklar;
using AkilliPazar.Application.Servisler;
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

    public class SepetServisi : ISepetServisi
    {
        private readonly SmartMarketDbContext _context;
        //private readonly IMapper _mapper;
        public SepetServisi(SmartMarketDbContext context)
        {
            _context = context;
           
        }

        
        public async Task<SepetToplamDTO> SepetGetir(string kullaniciId)
        {
            var sepet = await _context.Sepetler
                .Where(s => s.KullaniciId == kullaniciId)
                .Select(s => new SepetToplamDTO
                {
                    Id = s.Id,
                    UserId = s.KullaniciId,
                    SepetUrunleri = s.Urunler.Select(u => new SepetUrunListeleDTO
                    {
                        UrunId = u.UrunId,
                        Adet = u.Adet,
                        UrunAdi = u.Urun.Ad,
                        Fiyat = u.Urun.Fiyat                       
                    }).ToList()
                })
                .FirstOrDefaultAsync();

            if(sepet == null)
                return new SepetToplamDTO { UserId = kullaniciId };
            else 
                return sepet;
        }

        void ISepetServisi.SepeteEkle(string kullaniciId, SepeteEkleDTO dTO)
        {
           var sepet = _context.Sepetler.Include(x=>x.Urunler).FirstOrDefault(x=>x.KullaniciId==kullaniciId);
            if (sepet == null)
            {
                sepet = new Sepet { KullaniciId = kullaniciId };
                _context.Sepetler.Add(sepet);
                _context.SaveChanges();
            }

            var sepetUrun= sepet.Urunler.FirstOrDefault(x=>x.UrunId==dTO.UrunId);
            if (sepetUrun != null)
                sepetUrun.Adet += dTO.Adet;
            else
                _context.SepetUrunleri.Add(new SepetUrun
                {
                    SepetId = sepet.Id,
                    UrunId=dTO.UrunId,
                    Adet = dTO.Adet
                });
            _context.SaveChanges();
        }

        void ISepetServisi.SepettenKaldır(string kullaniciId, int urunId)
        {
            var sepet = _context.Sepetler.Include(x => x.Urunler).FirstOrDefault(x => x.KullaniciId == kullaniciId);
            if (sepet == null) return;
            var sepetUrun=sepet.Urunler.FirstOrDefault(x=>x.UrunId==urunId);
            if (sepetUrun != null)
            {
                _context.SepetUrunleri.Remove(sepetUrun);
                _context.SaveChanges();
            }
        }

        void ISepetServisi.SepetMiktarGuncelle(string kullaniciId, int urunId, int yeniAdet)
        {
            var sepet = _context.Sepetler.Include(x => x.Urunler).FirstOrDefault(x => x.KullaniciId == kullaniciId);
            if (sepet == null) return;

            var sepetUrun = sepet.Urunler.FirstOrDefault(x => x.UrunId == urunId);
            if (sepetUrun != null)
            {
                sepetUrun.Adet = yeniAdet;
                _context.SaveChanges();
            }
        }

        void ISepetServisi.SepetiTemizle(string kullaniciId)
        {
            var sepet = _context.Sepetler.Include(x => x.Urunler).FirstOrDefault(x => x.KullaniciId == kullaniciId);
            if (sepet == null) return;

            _context.SepetUrunleri.RemoveRange(sepet.Urunler);
            _context.SaveChanges();
        }
    }
}
