using EMS.DTOs;
using EMS.HelperClasses;
using EMS.Models;
using EMS.Repositories;
using EMS.Service;
using EMS.Enums;

namespace EMS.Service
{
    public interface ILeaveService
    {
        Task<ServiceResponse<Leave>> LeaveRequestAsync(AddLeaveDTO addLeaveDto);
        Task<ServiceResponse<IEnumerable<Leave>>> GetAllLeavesOfEmployeeAsync(int employeeId);
        Task<ServiceResponse<Leave>> ApproveOrRejectLeaveAsync(int leaveId, LeaveApprovalDTO leaveApprovalDto);
        Task<ServiceResponse<bool>> DeleteLeaveRequestAsync(int leaveId);
    }

    public class LeaveService : ILeaveService
    {
        private readonly IGenericDBRepository<Leave> _leaveService;
        private readonly IGenericDBRepository<Employee> _employeeService;

        public LeaveService(IGenericDBRepository<Leave> leaveService, IGenericDBRepository<Employee> employeeService)
        {
            _leaveService = leaveService;
            _employeeService = employeeService;
        }

        public async Task<ServiceResponse<Leave>> LeaveRequestAsync(AddLeaveDTO addLeaveDto)
        {
            // Get all existing leave records for the employee
            var existingLeaves = await _leaveService.GetByMultipleConditionsAsync(
                new List<FilterDTO>
                {
            new FilterDTO { PropertyName = "EmployeeId", Value = addLeaveDto.EmployeeId }
                }
            );


            foreach (var existingLeave in existingLeaves)
            {
                if (!(
                    (addLeaveDto.StartDate < existingLeave.StartDate && addLeaveDto.EndDate < existingLeave.StartDate) || // new leave is before the existing leave
                    (addLeaveDto.StartDate > existingLeave.EndDate && addLeaveDto.EndDate > existingLeave.EndDate)        // new leave is after the existing leave
                ))
                {
                    // If the new leave overlaps with any existing leave, return an error
                    return ServiceResponse<Leave>.FailureResponse("Leave request overlaps with an existing leave.", 400);
                }
            }
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

            var success = await _leaveService.AddAsync(newLeave);
            if (success)
                return ServiceResponse<Leave>.SuccessResponse(newLeave, "Leave request submitted successfully.", 201);

            return ServiceResponse<Leave>.FailureResponse("Failed to submit leave request.", 500);
        }

        public async Task<ServiceResponse<IEnumerable<Leave>>> GetAllLeavesOfEmployeeAsync(int employeeId)
        {
            var leaves = await _leaveService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "EmployeeId", Value = employeeId } }
            );

            if (!leaves.Any())
                return ServiceResponse<IEnumerable<Leave>>.FailureResponse("No leaves found for this employee.", 404);

            return ServiceResponse<IEnumerable<Leave>>.SuccessResponse(leaves, "Leaves fetched successfully.", 200);
        }

        public async Task<ServiceResponse<Leave>> ApproveOrRejectLeaveAsync(int leaveId, LeaveApprovalDTO leaveApprovalDto)
        {
            if (leaveApprovalDto == null)
                return ServiceResponse<Leave>.FailureResponse("Invalid input. Please provide valid approval details.", 400);

            var leaveRequest = await _leaveService.GetByIdAsync(leaveId);
            if (leaveRequest == null)
                return ServiceResponse<Leave>.FailureResponse("Leave request not found.", 404);

            if (leaveRequest.LeaveType != LeaveTypeEnum.UnpaidLeave && leaveApprovalDto.Status == StatusEnum.Approved)
            {
                var employee = await _employeeService.GetByIdAsync(leaveRequest.EmployeeId);
                employee.PaidLeavesRemaining -= leaveRequest.TotalDays;
                await _employeeService.UpdateAsync(employee);
            }

            leaveRequest.Status = leaveApprovalDto.Status;

            var success = await _leaveService.UpdateAsync(leaveRequest);
            if (success)
                return ServiceResponse<Leave>.SuccessResponse(leaveRequest, leaveApprovalDto.Status == StatusEnum.Approved ? "Leave approved successfully." : "Leave rejected successfully.", 200);

            return ServiceResponse<Leave>.FailureResponse("Failed to update leave status.", 500);
        }

        public async Task<ServiceResponse<bool>> DeleteLeaveRequestAsync(int leaveId)
        {
            var leave = await _leaveService.GetByIdAsync(leaveId);
            if (leave == null)
                return ServiceResponse<bool>.FailureResponse("Leave not found.", 404);

            if (leave.Status == StatusEnum.Approved || leave.Status == StatusEnum.Rejected)
                return ServiceResponse<bool>.FailureResponse("Leave request cannot be cancelled as it is already approved/rejected.", 400);

            var success = await _leaveService.DeleteAsync(leaveId);
            if (success)
                return ServiceResponse<bool>.SuccessResponse(true, "Leave request cancelled successfully.", 200);

            return ServiceResponse<bool>.FailureResponse("Failed to cancel leave request.", 500);
        }
    }
}
