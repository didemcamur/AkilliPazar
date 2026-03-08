using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Domain.Varliklar
{
    public class SepetUrun
    {
        [Key]
        public int Id { get; set; }

        public int SepetId { get; set; }

        public Sepet Sepet { get; set; } = null!;

        public int UrunId { get; set; }

        public Urun Urun { get; set; } = null!;

        public int Adet { get; set; } = 1; 
    }
}
