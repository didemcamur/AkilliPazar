using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Application.DTOs
{
    public class UrunGuncelleDTO
    {
        public int Id { get; set; }
        public string? Ad { get; set; }
        public string? Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int StokAdedi { get; set; }
        public int KategoriId { get; set; }
    }
}
