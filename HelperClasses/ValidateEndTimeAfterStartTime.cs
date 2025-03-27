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

            // Check if the object is AddTimeSheetDTO
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

            // Validate that EndTime is after StartTime
            if (startTime.HasValue && endTime <= startTime)
            {
                return new ValidationResult("EndTime must be after StartTime.");
            }
        }
        // If EndTime is a DateTime (used for leave requests)
        else if (value is DateTime endDate)
        {
            DateTime? startDate = null;

            // Check if the object is AddLeaveDTO
            if (validationContext.ObjectInstance is AddLeaveDTO addLeaveDto)
            {
                startDate = addLeaveDto.StartDate;
            }
            // Check if the object is UpdateLeaveDTO
            else if (validationContext.ObjectInstance is UpdateLeaveDTO updateLeaveDto)
            {
                startDate = updateLeaveDto.StartDate;
            }
            else
            {
                return new ValidationResult("Invalid object type for validation.");
            }

            // Validate that EndDate is after StartDate
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
