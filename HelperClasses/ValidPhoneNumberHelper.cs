using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace EMS.Helpers
{
    public class ValidPhoneNumberHelper : ValidationAttribute
    {
        private readonly string _pattern;

        // Constructor that allows an optional pattern parameter for phone number validation
        public ValidPhoneNumberHelper(string pattern = @"^\d{10}$") : base("Invalid phone number format.")
        {
            _pattern = pattern;
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var phoneNumber = value as string;

            if (string.IsNullOrEmpty(phoneNumber))
            {
                return ValidationResult.Success; // Skip validation if null or empty
            }

            // Check if the phone number matches the given pattern
            if (Regex.IsMatch(phoneNumber, _pattern))
            {
                return ValidationResult.Success; // Valid phone number
            }

            return new ValidationResult($"Phone number must match the pattern '{_pattern}'.");
        }
    }
}
