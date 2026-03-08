using AkilliPazar.Domain.Varliklar;
using AkilliPazar.Infrastructure;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Collections.Generic;

namespace AkilliPazar.Application.Servisler
{
    // 📌 Kullanıcı giriş yaptığında ona JWT üreten servis
    public class TokenServisi
    {
        private readonly IConfiguration _config;

        public TokenServisi(IConfiguration config)
        {
            _config = config;
        }

        public string TokenUret(ApplicationUser user,IList<string>roller)
        {
            // 1) Anahtarı al
            var keyString = _config["JwtAyarlar:Key"] ?? throw new InvalidOperationException("JWT Key yapılandırması bulunamadı.");
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(keyString)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // 2) Kullanıcıyla ilgili claim'leri hazırlıyoruz
            var claims = new List<Claim>
            {
                // ClaimTypes.NameIdentifier → Kullanıcının benzersiz Id bilgisi
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Email, user.Email ?? "")
                // TODO: İleride buraya rol bilgisi ekleyeceğiz (Admin/User)
            };
            //Roller claim'lere ekleniyor
            foreach (var rol in roller) 
            {
                claims.Add(new Claim(ClaimTypes.Role,rol));
            }

            // 3) Token nesnesini oluştur
            var expireMinutesString = _config["JwtAyarlar:ExpireMinutes"] ?? throw new InvalidOperationException("JWT ExpireMinutes yapılandırması bulunamadı.");
            var token = new JwtSecurityToken(
                issuer: _config["JwtAyarlar:Issuer"],
                audience: _config["JwtAyarlar:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(
                    int.Parse(expireMinutesString)
                ),
                signingCredentials: creds
            );

            // 4) Token'ı string olarak geri döndür
            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
