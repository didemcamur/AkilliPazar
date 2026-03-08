using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AkilliPazar.Domain.Varliklar
{
    public class Urun
    {
        public int Id { get; set; }
        public string? Ad { get; set; }
        public string? Aciklama { get; set; }
        public decimal Fiyat { get; set; }
        public int StokAdedi { get; set; }

        // Foreign Key
        public int KategoriId { get; set; }

        // Navigation Property - null! ekledim (CS8618 uyarısını önler)
        public Kategoriler Kategori { get; set; } = null!;

        public string? ResimYolu { get; set; }
    }
}