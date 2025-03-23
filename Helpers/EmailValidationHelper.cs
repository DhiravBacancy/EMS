using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EMS.Helpers
{
    public class ValidEmailHelper : ValidationAttribute
    {
        private readonly string _domain;

        // Constructor that allows an optional domain parameter
        public ValidEmailHelper(string domain = "@gmail.com") : base("Invalid email format.")
        {
            _domain = domain;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (string.IsNullOrEmpty(email))
            {
                return ValidationResult.Success; // Skip validation if null or empty
            }

            // Check if the email ends with the specified domain (default is @gmail.com)
            if (email.EndsWith(_domain, StringComparison.OrdinalIgnoreCase))
            {
                return ValidationResult.Success; // Valid email with the expected domain
            }

            return new ValidationResult($"Email must end with '{_domain}'.");
        }
    }
}
