using System;
using System.ComponentModel.DataAnnotations;

namespace EMS.Helpers
{
    public class ValidDateOfBirthHelper : ValidationAttribute
    {
        public int MinimumAge { get; set; } = 18;

        public ValidDateOfBirthHelper() : base("Invalid Date of Birth.")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var dateOfBirth = value as DateTime?;

            if (dateOfBirth == null)
            {
                return ValidationResult.Success;
            }

            if (dateOfBirth.Value > DateTime.Now)
            {
                return new ValidationResult("Date of Birth cannot be a future date.");
            }

            var age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (DateTime.Now < dateOfBirth.Value.AddYears(age)) age--; // Adjust if the birthday hasn't occurred yet this year

            if (age < MinimumAge)
            {
                return new ValidationResult($"User must be at least {MinimumAge} years old.");
            }

            return ValidationResult.Success; // Validation passed
        }
    }
}
