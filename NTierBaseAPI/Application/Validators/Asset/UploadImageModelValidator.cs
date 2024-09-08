using Application.Models.Asset;
using FluentValidation;
using Microsoft.AspNetCore.Http;
using MimeKit.Cryptography;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Application.Validators.Asset;
using System.Transactions;

namespace Application.Validators.Asset
{
    public class UploadImageModelValidator : AbstractValidator<UploadImageModel>
    {
        public UploadImageModelValidator()
        {
            RuleFor(p => p.File)
                .Must(BeValidFormat)
                .WithMessage(string.Format("File format is not valid! Allowed formats: {0}"
                    , string.Join(", ", AssetValidatorConfiguration.AllowFormats)))
                .Must(BeValidSize)
                .WithMessage(string.Format("File size is not valid! Must be less than or equal {0} MB"
                    , AssetValidatorConfiguration.MaxSizeInMb));
        }

        private bool BeValidFormat(IFormFile file)
        {
            string extension = Path.GetExtension(file.FileName);
            extension = extension.ToLower().Replace(".", "");
            return AssetValidatorConfiguration.AllowFormats.Contains(extension);
        }

        private bool BeValidSize(IFormFile file)
        {
            double sizeInMb = file.Length / 1024.0 / 1024.0;
            return sizeInMb <= AssetValidatorConfiguration.MaxSizeInMb;
        }
    }
}
