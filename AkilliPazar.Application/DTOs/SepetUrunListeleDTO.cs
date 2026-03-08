using AkilliPazar.Domain.Varliklar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Application.DTOs
{
    public class SepetUrunListeleDTO
    {
        public int UrunId { get; set; }
        public string? UrunAdi { get; set; }
        public int Adet {  get; set; }
        public decimal Fiyat { get; set; }

        // Hesaplanan Kolon 

        public decimal Toplam => Adet * Fiyat;
    }
}
