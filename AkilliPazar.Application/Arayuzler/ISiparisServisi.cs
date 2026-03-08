using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkilliPazar.Application.DTOs;

namespace AkilliPazar.Application.Arayuzler
{
    // Siparis servis arayuzu
    public interface ISiparisServisi
    {
        Task<SiparisListeleDTO> SiparisOlustur(string kullaniciId, SiparisOlusturDTO dto);
        Task<List<SiparisListeleDTO>> KullaniciSiparisleriniGetir(string kullaniciId);
        Task<SiparisListeleDTO?> SiparisDetayGetir(int siparisId, string kullaniciId);
        Task<List<SiparisListeleDTO>> TumSiparisleriGetir();
        Task<SiparisListeleDTO?> AdminSiparisDetayGetir(int siparisId);
        Task<bool> SiparisDurumGuncelle(int siparisId, SiparisDurumGuncelleDTO dto);
    }
}
