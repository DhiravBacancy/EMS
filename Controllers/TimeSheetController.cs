using EMS.DTOs;
using EMS.Helpers;
using EMS.Models;
using EMS.Service;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSheetController : ControllerBase
    {
        private readonly IGenericDBService<TimeSheet> _timeSheetService;

        public TimeSheetController(IGenericDBService<TimeSheet> timeSheetService)
        {
            _timeSheetService = timeSheetService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddTimeSheet([FromBody] AddTimeSheetDTO addTimeSheetDto)
        {

            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var newTimeSheet = new TimeSheet
            {
                EmployeeId = addTimeSheetDto.EmployeeId,
                Date = addTimeSheetDto.Date,
                StartTime = addTimeSheetDto.StartTime,
                EndTime = addTimeSheetDto.EndTime
            };

            var result = await _timeSheetService.AddAsync(newTimeSheet);
            return result
                ? Ok(new { Message = "Timesheet added successfully." })
                : StatusCode(500, new { Message = "Internal Server Error: Failed to add timesheet." });
        }


        // Get All TimeSheets of Employee
        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetTimeSheetsOfEmployee(int employeeId)
        {
            var timeSheets = await _timeSheetService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId }
            });

            return timeSheets != null && timeSheets.Any()
                ? Ok(timeSheets)
                : NotFound(new { Message = "No timesheets found for this employee." });
        }

        // Update TimeSheet
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTimeSheet(int id, [FromBody] UpdateTimeSheetDTO updateTimeSheetDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new { Message = "Invalid input. Please provide valid timesheet data." });

            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var timeSheet = await _timeSheetService.GetByIdAsync(id);
            if (timeSheet == null)
                return NotFound(new { Message = "Timesheet not found." });

            timeSheet.StartTime = updateTimeSheetDto.StartTime;
            timeSheet.EndTime = updateTimeSheetDto.EndTime;

            var result = await _timeSheetService.UpdateAsync(timeSheet);
            return result
                ? Ok(new { Message = "Timesheet updated successfully." })
                : StatusCode(500, new { Message = "Internal Server Error: Failed to update timesheet." });
        }


        // Delete TimeSheet
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTimeSheet(int id)
        {
            var timeSheet = await _timeSheetService.GetByIdAsync(id);
            if (timeSheet == null)
                return NotFound(new { Message = "Timesheet not found." });

            var result = await _timeSheetService.DeleteAsync(id);
            return result
                ? Ok(new { Message = "Timesheet deleted successfully." })
                : StatusCode(500, new { Message = "Internal Server Error: Failed to delete timesheet." });
        }
    }
}
