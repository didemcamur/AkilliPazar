using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using AkilliPazar.Domain.Varliklar;
using AkilliPazar.Application.DTOs;

namespace AkilliPazar.Application.Mapper
{
    public class AutoMapperProfil : Profile
    {
     public AutoMapperProfil() 
        {
            // Urun
            CreateMap<UrunEkleDTO, Urun>()
                .ForMember(d => d.ResimYolu, o => o.MapFrom(s => s.ResimYolu));
            CreateMap<UrunGuncelleDTO, Urun>();

            // Kategori
            CreateMap<KategoriEkleDTO, Kategoriler>();
            CreateMap<KategoriGuncelleDTO, Kategoriler>();

            // Sepet
            CreateMap<SepetUrun, SepetUrunListeleDTO>()
                .ForMember(d => d.UrunAdi, o => o.MapFrom(s => s.Urun.Ad))
                .ForMember(d => d.Fiyat, o => o.MapFrom(s => s.Urun.Fiyat));
            CreateMap<Sepet, SepetToplamDTO>();

            // Siparis
            CreateMap<SiparisOlusturDTO, Siparis>();
            CreateMap<SiparisUrun, SiparisUrunListeleDTO>();
            CreateMap<Siparis, SiparisListeleDTO>()
                .ForMember(d => d.KullaniciAdi, o => o.MapFrom(s => s.Kullanici != null ? s.Kullanici.AdSoyad : null))
                .ForMember(d => d.SiparisUrunleri, o => o.MapFrom(s => s.SiparisUrunleri));
        }
    }
}
