using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkilliPazar.Application.DTOs;
using AkilliPazar.Domain.Varliklar;


namespace AkilliPazar.Application.Arayuzler
{
    public interface IUrunServisi
    {
        IEnumerable<Urun> TumUrunleriGetir();
        Urun? IdyeGoreUrunGetir(int id);

        //DTO kullanarak urun ekleme
        void UrunEkle(UrunEkleDTO dto);
        //DTO kullanarak urun guncelleme    
        bool UrunGuncelle(UrunGuncelleDTO dto);
        bool UrunSil(int id);

    }
}
