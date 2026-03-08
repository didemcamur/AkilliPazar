using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FluentValidation;
using AkilliPazar.Application.DTOs;
namespace AkilliPazar.Application.Validators
{
    public class UrunEkleDTOValidator:AbstractValidator<UrunEkleDTO>
    {
        public UrunEkleDTOValidator()
        {
            RuleFor(x => x.Ad).NotEmpty().WithMessage("Ürün Adı Boş Bırakılamaz");

            RuleFor(x => x.Ad).MinimumLength(3).WithMessage("Ürün Adı en az 3 karakter olmalı");

            RuleFor(x => x.Fiyat).GreaterThan(0).WithMessage("Ürün Fiyatı 0 dan büyük olmalı");

            RuleFor(x => x.KategoriId).GreaterThan(0).WithMessage("Ürün Kategori Seçilmelidir");



        }
    }
}
