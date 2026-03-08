using AkilliPazar.API.Middlewares;
using AkilliPazar.Application.Arayuzler;
using AkilliPazar.Application.Servisler;
using AkilliPazar.Application.Validators;
using AkilliPazar.Infrastructure;
using AkilliPazar.Infrastructure.Servisler;
using AkilliPazar.Infrastructure.VeriTabani;
using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

// Servisleri Ekleyelim 
builder.Services.AddAutoMapper(typeof(AkilliPazar.Application.Mapper.AutoMapperProfil).Assembly);



// Controllers + JSON Circular Reference ��z�m�
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        // D�ng�sel referanslar� yoksay
        options.JsonSerializerOptions.ReferenceHandler =
            System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;

        // JSON'u daha okunabilir yap (iste�e ba�l�)
        options.JsonSerializerOptions.WriteIndented = true;

        // Null de�erleri g�sterme (iste�e ba�l�)
        options.JsonSerializerOptions.DefaultIgnoreCondition =
            System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

        // Property isimlerini camelCase'e çevir (JavaScript uyumluluğu için)
        options.JsonSerializerOptions.PropertyNamingPolicy = 
            System.Text.Json.JsonNamingPolicy.CamelCase;
    });

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// CORS ayarlari - React frontend icin
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Uygulama servislerimizi DI konteynerine ekleyelim
builder.Services.AddScoped<IUrunServisi, UrunServisi>();
builder.Services.AddScoped<IKategoriServisi, KategoriServisi>();
builder.Services.AddScoped<TokenServisi>();
builder.Services.AddScoped<ISepetServisi, SepetServisi>();
builder.Services.AddScoped<ISiparisServisi, SiparisServisi>(); // Siparis servisi eklendi
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly((typeof(UrunEkleDTOValidator).Assembly));
builder.Services.AddSwaggerGen(c =>
{
    // Temel Swagger/OpenAPI belgesi ayarlar�
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // *** Buras� Token Bilgilerini Ekleyen K�s�m ***
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "L�tfen 'Bearer ' kelimesi ile token bilgisini girin. �rn: 'Bearer eyJhbGciOiJIUzI1Ni...' ",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
    // *** Token Bilgilerini Ekleyen K�s�m Sonu ***

});
// ...

builder.Services.AddDbContext<SmartMarketDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("VeritabaniBaglantisi")));

builder.Services.AddIdentity<ApplicationUser, Microsoft.AspNetCore.Identity.IdentityRole>()
    .AddEntityFrameworkStores<SmartMarketDbContext>()
    .AddDefaultTokenProviders();

// jwt servislerini ekleyelim
var keyString = builder.Configuration["JwtAyarlar:Key"] ?? throw new InvalidOperationException("JWT Key yapılandırması bulunamadı.");
var key = Encoding.UTF8.GetBytes(keyString);
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,

        ValidAudience = builder.Configuration["JwtAyarlar:Audience"],
        ValidIssuer = builder.Configuration["JwtAyarlar:Issuer"],
        IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(key)
    };
});



var app = builder.Build();

// Seed verilerini olustur (roller, kullanicilar, kategoriler, urunler)
using (var scope = app.Services.CreateScope())
{
    try
    {
        await AkilliPazar.Infrastructure.VeriTabani.SeedData.SeedVerileriniOlustur(app.Services);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[HATA] Seed verileri olusturulurken hata: {ex.Message}");
    }
}

// http istek hatti
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowReactApp"); // CORS middleware'i
app.UseMiddleware<ExceptionMiddleware>();
app.UseAuthentication();
app.UseStaticFiles();// buraya UseAuthentication ekledik
app.UseAuthorization();
app.MapControllers();
app.Run();