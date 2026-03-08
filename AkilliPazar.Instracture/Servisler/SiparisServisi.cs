using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.DTOs;
using AkilliPazar.Domain.Varliklar;
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
    // Siparis islemlerini yoneten servis
    public class SiparisServisi : ISiparisServisi
    {
        private readonly SmartMarketDbContext _context;
        private readonly IMapper _mapper;

        public SiparisServisi(SmartMarketDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // Sepetteki urunleri siparise donustur
        public async Task<SiparisListeleDTO> SiparisOlustur(string kullaniciId, SiparisOlusturDTO dto)
        {
            var sepet = await _context.Sepetler
                .Include(s => s.Urunler)
                    .ThenInclude(su => su.Urun)
                .FirstOrDefaultAsync(s => s.KullaniciId == kullaniciId);

            if (sepet == null || !sepet.Urunler.Any())
                throw new InvalidOperationException("Sepetiniz bos, siparis olusturulamaz.");

            // Stok kontrolu
            foreach (var sepetUrun in sepet.Urunler)
            {
                if (sepetUrun.Urun.StokAdedi < sepetUrun.Adet)
                {
                    throw new InvalidOperationException(
                        $"'{sepetUrun.Urun.Ad}' icin yetersiz stok. Istenen: {sepetUrun.Adet}, Mevcut: {sepetUrun.Urun.StokAdedi}");
                }
            }

            decimal toplamTutar = sepet.Urunler.Sum(u => u.Adet * u.Urun.Fiyat);

            var siparis = _mapper.Map<Siparis>(dto);
            siparis.KullaniciId = kullaniciId;
            siparis.OlusturmaTarihi = DateTime.Now;
            siparis.Durum = SiparisDurumu.Beklemede;
            siparis.ToplamTutar = toplamTutar;

            _context.Siparisler.Add(siparis);
            await _context.SaveChangesAsync();

            // Sepet urunlerini siparis urunlerine donustur ve stok dusur
            foreach (var sepetUrun in sepet.Urunler)
            {
                var siparisUrun = new SiparisUrun
                {
                    SiparisId = siparis.Id,
                    UrunId = sepetUrun.UrunId,
                    Adet = sepetUrun.Adet,
                    BirimFiyat = sepetUrun.Urun.Fiyat,
                    UrunAdi = sepetUrun.Urun.Ad ?? "Urun"
                };
                _context.SiparisUrunleri.Add(siparisUrun);

                // Stok dusur
                sepetUrun.Urun.StokAdedi -= sepetUrun.Adet;
            }

            // Sepeti temizle
            _context.SepetUrunleri.RemoveRange(sepet.Urunler);
            await _context.SaveChangesAsync();

            return await SiparisDetayGetir(siparis.Id, kullaniciId)
                   ?? throw new InvalidOperationException("Siparis olusturuldu ancak detaylar alinamadi.");
        }

        // Kullanicinin siparislerini getir
        public async Task<List<SiparisListeleDTO>> KullaniciSiparisleriniGetir(string kullaniciId)
        {
            var siparisler = await _context.Siparisler
                .Where(s => s.KullaniciId == kullaniciId)
                .OrderByDescending(s => s.OlusturmaTarihi)
                .Include(s => s.SiparisUrunleri)
                .ToListAsync();

            return _mapper.Map<List<SiparisListeleDTO>>(siparisler);
        }

        // Siparis detayi getir - Kullanici sadece kendininkileri gorebilir
        public async Task<SiparisListeleDTO?> SiparisDetayGetir(int siparisId, string kullaniciId)
        {
            var siparis = await _context.Siparisler
                .Where(s => s.Id == siparisId && s.KullaniciId == kullaniciId)
                .Include(s => s.SiparisUrunleri)
                .FirstOrDefaultAsync();

            if (siparis == null)
                return null;

            return _mapper.Map<SiparisListeleDTO>(siparis);
        }

        // Tum siparisleri getir - Admin
        public async Task<List<SiparisListeleDTO>> TumSiparisleriGetir()
        {
            var siparisler = await _context.Siparisler
                .OrderByDescending(s => s.OlusturmaTarihi)
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisUrunleri)
                .ToListAsync();

            return _mapper.Map<List<SiparisListeleDTO>>(siparisler);
        }

        // Siparis detayi getir - Admin
        public async Task<SiparisListeleDTO?> AdminSiparisDetayGetir(int siparisId)
        {
            var siparis = await _context.Siparisler
                .Where(s => s.Id == siparisId)
                .Include(s => s.Kullanici)
                .Include(s => s.SiparisUrunleri)
                .FirstOrDefaultAsync();

            if (siparis == null)
                return null;

            return _mapper.Map<SiparisListeleDTO>(siparis);
        }

        // Siparis durumunu guncelle - Admin
        public async Task<bool> SiparisDurumGuncelle(int siparisId, SiparisDurumGuncelleDTO dto)
        {
            var siparis = await _context.Siparisler
                .Include(s => s.SiparisUrunleri)
                    .ThenInclude(su => su.Urun)
                .FirstOrDefaultAsync(s => s.Id == siparisId);

            if (siparis == null)
                return false;

            // Iptal edilirse stoklari geri ekle
            if (dto.YeniDurum == SiparisDurumu.IptalEdildi && siparis.Durum != SiparisDurumu.IptalEdildi)
            {
                foreach (var siparisUrun in siparis.SiparisUrunleri)
                {
                    if (siparisUrun.Urun != null)
                        siparisUrun.Urun.StokAdedi += siparisUrun.Adet;
                }
            }

            siparis.Durum = dto.YeniDurum;
            siparis.GuncellemeTarihi = DateTime.Now;

            await _context.SaveChangesAsync();
            return true;
        }
    }
}
