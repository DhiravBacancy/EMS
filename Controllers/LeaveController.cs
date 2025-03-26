using EMS.DTOs;
using EMS.Helpers;
using EMS.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [Authorize]
    public class LeaveController : ControllerBase
    {
        private readonly ILeaveService _leaveService;

        public LeaveController(ILeaveService leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpPost]
        public async Task<IActionResult> LeaveRequest([FromBody] AddLeaveDTO addLeaveDto)
        {
            var response = await _leaveService.LeaveRequestAsync(addLeaveDto);
            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(new { message = response.Message });
        }


        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAllLeavesOfEmployee(int employeeId)
        {
            var response = await _leaveService.GetAllLeavesOfEmployeeAsync(employeeId);
            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(response.Data);
        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{leaveId}")]
        public async Task<IActionResult> ApproveOrRejectLeave(int leaveId, [FromBody] LeaveApprovalDTO leaveApprovalDto)
        {
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var response = await _leaveService.ApproveOrRejectLeaveAsync(leaveId, leaveApprovalDto);
            if (!response.Success)
                return BadRequest(new { message = response.Message });

            return Ok(new { message = response.Message });
        }


        [HttpDelete("{leaveId}")]
        public async Task<IActionResult> DeleteLeaveRequest(int leaveId)
        {
            var response = await _leaveService.DeleteLeaveRequestAsync(leaveId);
            if (!response.Success)
                return NotFound(new { message = response.Message });

            return Ok(new { message = response.Message });
        }
    }
}
