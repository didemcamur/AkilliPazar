using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkilliPazar.Infrastructure;

namespace AkilliPazar.Domain.Varliklar
{
    // Siparis tablosu
    public class Siparis
    {
        [Key]
        public int Id { get; set; }

        public string KullaniciId { get; set; } = null!;
        public ApplicationUser Kullanici { get; set; } = null!;

        public DateTime OlusturmaTarihi { get; set; } = DateTime.Now;
        public DateTime? GuncellemeTarihi { get; set; }

        public SiparisDurumu Durum { get; set; } = SiparisDurumu.Beklemede;
        public decimal ToplamTutar { get; set; }

        public string? TeslimatAdresi { get; set; }
        public string? SiparisNotu { get; set; }

        public List<SiparisUrun> SiparisUrunleri { get; set; } = new();
    }
}
