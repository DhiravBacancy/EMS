using System;
using System.ComponentModel.DataAnnotations;
using EMS.DTOs;

namespace EMS.Helpers
{
    // Custom validation attribute to ensure EndTime > StartTime
    public class EndTimeAfterStartTimeHelper : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            // Access the AddTimeSheetDTO object from the validation context
            var timeSheet = (AddTimeSheetDTO)validationContext.ObjectInstance;

            // Check if EndTime is greater than StartTime
            if (timeSheet.EndTime != null && timeSheet.EndTime <= timeSheet.StartTime)
            {
                return new ValidationResult("EndTime must be greater than StartTime.");
            }


            return ValidationResult.Success; // Validation passes
        }
    }
}
