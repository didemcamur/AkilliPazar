using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Application.DTOs
{
    public class SepetToplamDTO
    {
        public int Id { get; set; }
        public string UserId { get; set; } = null!;
        public List<SepetUrunListeleDTO>? SepetUrunleri { get; set; } = new();

        public decimal GenelToplam => SepetUrunleri?.Sum(x => x.Toplam) ?? 0;
    }
}
