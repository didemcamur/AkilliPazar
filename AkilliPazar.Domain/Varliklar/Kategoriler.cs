using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AkilliPazar.Domain.Varliklar
{
    public class Kategoriler
    {
        [Key]  // Bu attribute ile PK belirtiyoruz
        public int Id { get; set; }  // İsim farklı olabilir
        public string Ad { get; set; } = string.Empty;

        [JsonIgnore]  //=> İlişkisel sorgulamada sonzuz döngüyü engeller.
        public ICollection<Urun> Urunler { get; set; } = new List<Urun>();
    }
}