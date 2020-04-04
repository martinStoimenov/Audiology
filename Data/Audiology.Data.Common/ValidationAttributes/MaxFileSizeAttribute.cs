namespace Audiology.Data.Common.ValidationAttributes
{
    using System.ComponentModel.DataAnnotations;

    using Microsoft.AspNetCore.Http;

    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            this.maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(
        object value, ValidationContext validationContext)
        {
            var file = value as IFormFile;

            //var extension = Path.GetExtension(file.FileName);
            //var allowedExtensions = new[] { ".jpg", ".png" };`enter code here`

            if (file != null)
            {
                if (file.Length > maxFileSize)
                {
                    return new ValidationResult($"Maximum allowed file size is {this.maxFileSize} bytes.");
                }
            }

            return ValidationResult.Success;
        }
    }
}
