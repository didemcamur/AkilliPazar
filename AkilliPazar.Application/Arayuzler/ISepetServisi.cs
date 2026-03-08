using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkilliPazar.Application.DTOs;

namespace AkilliPazar.Application.Arayuzler
{
    public interface ISepetServisi
    {
        void SepeteEkle(string kullaniciId, SepeteEkleDTO dTO);
        Task<SepetToplamDTO> SepetGetir(string kullaniciId);
        void SepettenKaldır(string kullaniciId, int urunId);
        void SepetMiktarGuncelle(string kullaniciId, int urunId, int yeniAdet);
        void SepetiTemizle(string kullaniciId);
    }
}
