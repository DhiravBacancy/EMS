using EMS.DTOs;
using System.ComponentModel.DataAnnotations;

public class EndTimeAfterStartTimeHelper : ValidationAttribute
{
    protected override ValidationResult IsValid(object value, ValidationContext validationContext)
    {
        // If EndTime is a TimeSpan (used for timesheets)
        if (value is TimeSpan endTime)
        {
            TimeSpan? startTime = null;

            if (validationContext.ObjectInstance is AddTimeSheetDTO addTimeSheet)
            {
                startTime = addTimeSheet.StartTime;
            }
            // Check if the object is UpdateTimeSheetDTO
            else if (validationContext.ObjectInstance is UpdateTimeSheetDTO updateTimeSheet)
            {
                startTime = updateTimeSheet.StartTime;
            }
            else
            {
                return new ValidationResult("Invalid object type for validation.");
            }

            if (startTime.HasValue && endTime <= startTime)
            {
                return new ValidationResult("EndTime must be after StartTime.");
            }
        }
        else if (value is DateTime endDate)
        {
            DateTime? startDate = null;

            if (validationContext.ObjectInstance is AddLeaveDTO addLeaveDto)
            {
                startDate = addLeaveDto.StartDate;
            }
            else if (validationContext.ObjectInstance is UpdateLeaveDTO updateLeaveDto)
            {
                startDate = updateLeaveDto.StartDate;
            }
            else
            {
                return new ValidationResult("Invalid object type for validation.");
            }

            if (startDate.HasValue && endDate <= startDate)
            {
                return new ValidationResult("EndDate must be after StartDate.");
            }
        }
        else
        {
            return new ValidationResult("Invalid object type for validation.");
        }

        return ValidationResult.Success;
    }
}
