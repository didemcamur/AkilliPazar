using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;




namespace AkilliPazar.Application.DTOs
{
    public class UrunEkleDTO
    {
        public string? Ad { get; set; }
        public string? Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int StokAdedi { get; set; }
        public int  KategoriId { get; set; }
        
        public IFormFile? Resim { get; set; }

        public string? ResimYolu { get; set; }
        
    }
}
