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

        // Leave Request
        [HttpPost]
        public async Task<IActionResult> LeaveRequest([FromBody] AddLeaveDTO addLeavesDto)
        {
            
            // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            // Check if leave already exists for the given employee and start date
            var existingLeave = (await _leaveService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = addLeavesDto.EmployeeId },
                new FilterDTO { PropertyName = "StartDate", Value = addLeavesDto.StartDate }
            })).FirstOrDefault();

            if (existingLeave != null)
                return BadRequest(new { message = "Leave already exists for the given employee and start date." });

            // Create new leave request
            var newLeave = new Leave
            {
                EmployeeId = addLeavesDto.EmployeeId,
                StartDate = addLeavesDto.StartDate,
                EndDate = addLeavesDto.EndDate,
                LeaveType = addLeavesDto.LeaveType,
                Status = StatusEnum.Pending, // assuming default status is "Pending"
                Reason = addLeavesDto.Reason,
                AppliedAt = DateTime.UtcNow
            };

            // Add leave request to the service
            if (await _leaveService.AddAsync(newLeave))
                return Ok(new { message = "Leave request submitted successfully." });
                
            return StatusCode(500, new { message = "Failed to submit leave request due to an internal server error." });
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAllLeavesOfEmployee(int employeeId)
        {
            var leavesOfEmployee = (await _leaveService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId }
            })).FirstOrDefault();

            if (leavesOfEmployee == null)
                return NotFound(new { message = "No leaves found." });
            return Ok(leavesOfEmployee);
        }

        [HttpPut("{employeeId}/{leaveId}")]
        public async Task<IActionResult> ApproveOrRejectLeave(int employeeId, int leaveId, [FromBody] LeaveApprovalDTO leaveApprovalDto)
        {
            if (leaveApprovalDto == null)
                return BadRequest(new { message = "Invalid input. Please provide valid approval details." });

                    // Call the validation helper method to check for ModelState errors
            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;


            // Fetch the leave request for the given leaveId and employeeId
            var leaveRequest = (await _leaveService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId },
                new FilterDTO { PropertyName = "LeaveId", Value = leaveId }
            })).FirstOrDefault();

            if (leaveRequest == null)
                return NotFound(new { message = "Leave request not found for the given Employee and Leave ID." });

            // Update the status of the leave based on the approval or rejection decision
            leaveRequest.Status = leaveApprovalDto.Status;

            if (await _leaveService.UpdateAsync(leaveRequest))
                return Ok(new { message = leaveApprovalDto.Status == StatusEnum.Approved? "Leave approved successfully." : "Leave rejected successfully." });

            return StatusCode(500, new { message = "Failed to update leave status due to an internal server error." });
        }




        [HttpDelete]
        public async Task<IActionResult> DeleteLeaveRequest(int leaveId)
        {
            var leave = await _leaveService.GetByIdAsync(leaveId);
            if (leave == null)
                return NotFound(new { message = "Leave not found." });
            if (leave.Status == StatusEnum.Approved || leave.Status == StatusEnum.Rejected)
                return BadRequest(new { message = "Leave request cannot be cancelled as it is already approved/rejected." });
            if (await _leaveService.DeleteAsync(leaveId))
                return Ok(new { message = "Leave request cancelled successfully." });
            return StatusCode(500, new { message = "Failed to cancel leave request due to an internal server error." });
        }

        

    }
}
