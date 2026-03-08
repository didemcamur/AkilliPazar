using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkilliPazar.Domain.Varliklar;

namespace AkilliPazar.Application.DTOs
{
    // Siparis listeleme icin DTO
    public class SiparisListeleDTO
    {
        public int Id { get; set; }
        public DateTime OlusturmaTarihi { get; set; }
        public DateTime? GuncellemeTarihi { get; set; }
        public SiparisDurumu Durum { get; set; }
        public string DurumAdi => Durum.ToString();
        public decimal ToplamTutar { get; set; }
        public string? TeslimatAdresi { get; set; }
        public string? SiparisNotu { get; set; }
        public string? KullaniciId { get; set; }
        public string? KullaniciAdi { get; set; }
        public List<SiparisUrunListeleDTO> SiparisUrunleri { get; set; } = new();
    }
}
