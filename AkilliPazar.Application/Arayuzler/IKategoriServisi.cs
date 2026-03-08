using AkilliPazar.Application.DTOs;
using AkilliPazar.Domain.Varliklar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Application.Arayuzler
{
    public interface IKategoriServisi
    {
        IEnumerable<Kategoriler> TumKategorileriGetir();

        Kategoriler? IdyeGoreKategoriGetir(int id);

        void KategoriEkle(KategoriEkleDTO dto);
        void KategoriGuncelle(KategoriGuncelleDTO dto);
        void KategoriSil(int id);

        IEnumerable<Urun> KategoriyeGoreUrunleriGetir(int kategoriId);
    }
}
