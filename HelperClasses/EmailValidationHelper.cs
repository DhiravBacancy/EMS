using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EMS.Helpers
{
    public class ValidEmailHelper : ValidationAttribute
    {
        private readonly string _domain;

        public ValidEmailHelper(string domain = "@gmail.com") : base("Invalid email format.")
        {
            _domain = domain;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (string.IsNullOrEmpty(email))
            {
                return ValidationResult.Success;
            }

            if (email.EndsWith(_domain, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Email must end with '{_domain}'.");
        }
    }
}
