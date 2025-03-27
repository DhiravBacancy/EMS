using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EMS.Helpers
{
    public class ValidPhoneNumberHelper : ValidationAttribute
    {
        private readonly string _pattern;

        public ValidPhoneNumberHelper(string pattern = @"^\d{10}$") : base("Invalid phone number format.")
        {
            _pattern = pattern;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumber = value as string;

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return ValidationResult.Success;
            }

            if (Regex.IsMatch(phoneNumber, _pattern))
            {
                return ValidationResult.Success;
            }

            return new ValidationResult($"Phone number must match the pattern '{_pattern}'.");
        }
    }
}
