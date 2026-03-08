using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Application.DTOs
{
    // Siparis urunu listeleme icin DTO
    public class SiparisUrunListeleDTO
    {
        public int UrunId { get; set; }
        public string UrunAdi { get; set; } = null!;
        public int Adet { get; set; }
        public decimal BirimFiyat { get; set; }
        public decimal Toplam => Adet * BirimFiyat;
    }
}
