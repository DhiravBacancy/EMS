using EMS.DTOs;
using EMS.Helpers;
using EMS.Models;
using EMS.Service;
using EMS.Services;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TimeSheetController : ControllerBase
    {
        private readonly IGenericDBService<TimeSheet> _timeSheetService;

        public TimeSheetController(IGenericDBService<TimeSheet> timeSheetService, IExportTimesheetsToExcelService exportToExcelService)
        {
            _timeSheetService = timeSheetService;
        }

        [HttpPost("add")]
        public async Task<IActionResult> AddTimeSheet([FromBody] AddTimeSheetDTO addTimeSheetDto)
        {
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var existingTimeSheet = (await _timeSheetService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = addTimeSheetDto.EmployeeId },
                new FilterDTO { PropertyName = "Date", Value = addTimeSheetDto.Date }
            })).FirstOrDefault();

            if (existingTimeSheet != null)
                return BadRequest(new { Message = "Timesheet already exists for the given date." });

            var newTimeSheet = new TimeSheet
            {
                EmployeeId = addTimeSheetDto.EmployeeId,
                Date = addTimeSheetDto.Date,
                StartTime = addTimeSheetDto.StartTime,
                EndTime = addTimeSheetDto.EndTime,
                Description = addTimeSheetDto.Description
            };

            return await _timeSheetService.AddAsync(newTimeSheet)
                ? Ok(new { Message = "Timesheet added successfully." })
                : StatusCode(500, new { Message = "Internal Server Error: Failed to add timesheet." });
        }

        [HttpGet("employee/{employeeId}")]
        public async Task<IActionResult> GetTimeSheetsOfEmployee(int employeeId)
        {
            var timeSheets = await _timeSheetService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId }
            });

            return timeSheets.Any()
                ? Ok(timeSheets)
                : NotFound(new { Message = "No timesheets found for this employee." });
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateTimeSheet(int id, [FromBody] UpdateTimeSheetDTO updateTimeSheetDto)
        {
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var timeSheet = await _timeSheetService.GetByIdAsync(id);
            if (timeSheet == null)
                return NotFound(new { Message = "Timesheet not found." });

            timeSheet.StartTime = updateTimeSheetDto.StartTime ?? TimeSpan.Zero;
            timeSheet.EndTime = updateTimeSheetDto.EndTime ?? TimeSpan.Zero;

            return await _timeSheetService.UpdateAsync(timeSheet)
                ? Ok(new { Message = "Timesheet updated successfully." })
                : StatusCode(500, new { Message = "Internal Server Error: Failed to update timesheet." });
        }

        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteTimeSheet(int id)
        {
            var timeSheet = await _timeSheetService.GetByIdAsync(id);
            if (timeSheet == null)
                return NotFound(new { Message = "Timesheet not found." });

            return await _timeSheetService.DeleteAsync(id)
                ? Ok(new { Message = "Timesheet deleted successfully." })
                : StatusCode(500, new { Message = "Internal Server Error: Failed to delete timesheet." });
        }
    }
}
