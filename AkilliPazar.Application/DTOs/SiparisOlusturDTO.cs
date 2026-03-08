using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Application.DTOs
{
    // Siparis olusturma icin DTO
    public class SiparisOlusturDTO
    {
        public string TeslimatAdresi { get; set; } = null!;
        public string? SiparisNotu { get; set; }
    }
}
