using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;


namespace AkilliPazar.Infrastructure
{
    public class ApplicationUser: IdentityUser
    {
       

            public string AdSoyad { get; set; } = null!;

        

    }
}
