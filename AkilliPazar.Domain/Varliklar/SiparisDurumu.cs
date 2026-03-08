using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AkilliPazar.Domain.Varliklar
{
    // Siparis durumlari
    public enum SiparisDurumu
    {
        Beklemede = 0,
        Hazirlaniyor = 1,
        Kargolandi = 2,
        TeslimEdildi = 3,
        Tamamlandi = 4,
        IptalEdildi = 5
    }
}
