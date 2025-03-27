using EMS.DTOs;
using EMS.Helpers;
using EMS.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class TimeSheetController : ControllerBase
    {
        private readonly ITimeSheetService _timeSheetService;

        public TimeSheetController(ITimeSheetService timeSheetService)
        {
            _timeSheetService = timeSheetService;
        }

        [HttpPost]
        public async Task<IActionResult> AddTimeSheet([FromBody] AddTimeSheetDTO addTimeSheetDto)
        {
            var response = await _timeSheetService.AddTimeSheetAsync(addTimeSheetDto);
            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(new { message = response.Message });
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAllTimeSheetsOfEmployee(int employeeId)
        {
            var response = await _timeSheetService.GetAllTimeSheetsOfEmployeeAsync(employeeId);
            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateTimeSheet(int id, [FromBody] UpdateTimeSheetDTO updateTimeSheetDto)
        {
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var response = await _timeSheetService.UpdateTimeSheetAsync(id, updateTimeSheetDto);
            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(new { message = response.Message });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTimeSheet(int id)
        {
            var response = await _timeSheetService.DeleteTimeSheetAsync(id);
            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(new { message = response.Message });
        }
    }
}
