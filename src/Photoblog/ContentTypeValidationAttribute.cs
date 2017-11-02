using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Photoblog {
    public class ContentTypeValidationAttribute : ValidationAttribute {

        public string[] AllowedTypes { get; }

        public ContentTypeValidationAttribute(params string[] allowedTypes) {
            AllowedTypes = allowedTypes;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) {
            var formFile = value as IFormFile;
            IFormFile invalidFile = null;

            if (formFile != null) {
                if (!IsFileValid(formFile)) {
                    invalidFile = formFile;
                }
            } else {
                // If it's a not a single file, it can also be a collection of files
                var formFiles = value as IEnumerable<IFormFile>;
                if (formFiles == null) {
                    return new ValidationResult($"{validationContext.MemberName} is not a file.");
                }

                invalidFile = formFiles.FirstOrDefault(f => !IsFileValid(f));
            }

            if (invalidFile != null) {
                return new ValidationResult($"Content type [{invalidFile.ContentType}] is not allowed.");
            }

            return ValidationResult.Success;
        }

        bool IsFileValid(IFormFile formFile) => AllowedTypes.Contains(formFile.ContentType);


    }
}
