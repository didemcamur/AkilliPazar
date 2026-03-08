using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Domain.Varliklar
{
    // Siparis-Urun ara tablosu
    public class SiparisUrun
    {
        [Key]
        public int Id { get; set; }

        public int SiparisId { get; set; }
        public Siparis Siparis { get; set; } = null!;

        public int UrunId { get; set; }
        public Urun Urun { get; set; } = null!;

        public int Adet { get; set; } = 1;

        // Siparis anindaki fiyat
        public decimal BirimFiyat { get; set; }

        // Siparis anindaki urun adi
        public string UrunAdi { get; set; } = null!;

        public decimal Toplam => Adet * BirimFiyat;
    }
}
