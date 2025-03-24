using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using EMS.DTOs;
using EMS.Models;
using EMS.Enums;
using EMS.Helpers;

namespace EMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class LeaveController : ControllerBase
    {
        private readonly IGenericDBService<Leave> _leaveService;

        public LeaveController(IGenericDBService<Leave> leaveService)
        {
            _leaveService = leaveService;
        }

        [HttpPost]
        public async Task<IActionResult> LeaveRequest([FromBody] AddLeaveDTO addLeaveDto)
        {
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var existingLeave = (await _leaveService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = addLeaveDto.EmployeeId },
                new FilterDTO { PropertyName = "StartDate", Value = addLeaveDto.StartDate }
            })).FirstOrDefault();

            if (existingLeave != null)
                return BadRequest(new { message = "Leave request already exists for the given employee and start date." });

            var newLeave = new Leave
            {
                EmployeeId = addLeaveDto.EmployeeId,
                StartDate = addLeaveDto.StartDate,
                EndDate = addLeaveDto.EndDate,
                LeaveType = addLeaveDto.LeaveType,
                Status = StatusEnum.Pending,
                Reason = addLeaveDto.Reason,
                AppliedAt = DateTime.UtcNow
            };

            return await _leaveService.AddAsync(newLeave)
                ? Ok(new { message = "Leave request submitted successfully." })
                : StatusCode(500, new { message = "Failed to submit leave request due to an internal server error." });
        }

        [HttpGet("{employeeId}")]
        public async Task<IActionResult> GetAllLeavesOfEmployee(int employeeId)
        {
            var leaves = await _leaveService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId }
            });

            return leaves.Any()
                ? Ok(leaves)
                : NotFound(new { message = "No leaves found for this employee." });
        }

        [HttpPut("{leaveId}")]
        public async Task<IActionResult> ApproveOrRejectLeave(int leaveId, [FromBody] LeaveApprovalDTO leaveApprovalDto)
        {
            if (leaveApprovalDto == null)
                return BadRequest(new { message = "Invalid input. Please provide valid approval details." });

            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var leaveRequest = await _leaveService.GetByIdAsync(leaveId);
            if (leaveRequest == null)
                return NotFound(new { message = "Leave request not found." });

            leaveRequest.Status = leaveApprovalDto.Status;

            return await _leaveService.UpdateAsync(leaveRequest)
                ? Ok(new { message = leaveApprovalDto.Status == StatusEnum.Approved ? "Leave approved successfully." : "Leave rejected successfully." })
                : StatusCode(500, new { message = "Failed to update leave status due to an internal server error." });
        }

        [HttpDelete("{leaveId}")]
        public async Task<IActionResult> DeleteLeaveRequest(int leaveId)
        {
            var leave = await _leaveService.GetByIdAsync(leaveId);
            if (leave == null)
                return NotFound(new { message = "Leave not found." });

            if (leave.Status == StatusEnum.Approved || leave.Status == StatusEnum.Rejected)
                return BadRequest(new { message = "Leave request cannot be cancelled as it is already approved/rejected." });

            return await _leaveService.DeleteAsync(leaveId)
                ? Ok(new { message = "Leave request cancelled successfully." })
                : StatusCode(500, new { message = "Failed to cancel leave request due to an internal server error." });
        }
    }
}