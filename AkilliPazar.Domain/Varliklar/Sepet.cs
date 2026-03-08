using AkilliPazar.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AkilliPazar.Domain.Varliklar;

namespace AkilliPazar.Domain.Varliklar
{
    public class Sepet
    {
        [Key]
        public int Id { get; set; }

        public string KullaniciId { get; set; } = null!;

        public ApplicationUser Kullanici { get; set; } = null!;

        public List<SepetUrun> Urunler { get; set; } = new(); 
    }
}
