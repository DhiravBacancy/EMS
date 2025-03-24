using System;
using System.ComponentModel.DataAnnotations;

namespace EMS.Helpers
{
    public class ValidDateOfBirthHelper : ValidationAttribute
    {
        // Default minimum age is 18 if no custom age is provided
        public int MinimumAge { get; set; } = 18;

        public ValidDateOfBirthHelper() : base("Invalid Date of Birth.")
        {
        }

        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // If the value is null, we skip validation (it can be handled by the Required attribute elsewhere)
            var dateOfBirth = value as DateTime?;

            if (dateOfBirth == null)
            {
                return ValidationResult.Success;
            }

            // Check if the Date of Birth is in the future
            if (dateOfBirth.Value > DateTime.Now)
            {
                return new ValidationResult("Date of Birth cannot be a future date.");
            }

            // Calculate the age
            var age = DateTime.Now.Year - dateOfBirth.Value.Year;
            if (DateTime.Now < dateOfBirth.Value.AddYears(age)) age--; // Adjust if the birthday hasn't occurred yet this year

            // If age is less than the provided or default minimum age, return an error
            if (age < MinimumAge)
            {
                return new ValidationResult($"User must be at least {MinimumAge} years old.");
            }

            return ValidationResult.Success; // Validation passed
        }
    }
}
