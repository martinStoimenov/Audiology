namespace Audiology.Data.Common.ValidationAttributes
{
    using System.ComponentModel.DataAnnotations;
    using System.IO;
    using System.Linq;

    using Microsoft.AspNetCore.Http;

    public class AllowedExtensionsAttribute : ValidationAttribute
    {
        private readonly string[] extensions;

        public AllowedExtensionsAttribute(string[] Extensions)
        {
            this.extensions = Extensions;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;
            var extension = Path.GetExtension(file.FileName);
            if (!(file == null))
            {
                if (!this.extensions.Contains(extension.ToLower()))
                {
                    return new ValidationResult($"This song extension is not allowed!");
                }
            }

            return ValidationResult.Success;
        }
    }
}
