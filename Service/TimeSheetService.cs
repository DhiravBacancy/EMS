using EMS.DTOs;
using EMS.HelperClasses;
using EMS.Models;
using EMS.Repositories;
using EMS.Services;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMS.Services
{
    public interface ITimeSheetService
    {
        Task<ServiceResponse<TimeSheet>> AddTimeSheetAsync(AddTimeSheetDTO addTimeSheetDto);
        Task<ServiceResponse<IEnumerable<TimeSheet>>> GetAllTimeSheetsOfEmployeeAsync(int employeeId);
        Task<ServiceResponse<TimeSheet>> UpdateTimeSheetAsync(int id, UpdateTimeSheetDTO updateTimeSheetDto);
        Task<ServiceResponse<bool>> DeleteTimeSheetAsync(int id);
    }

    public class TimeSheetService : ITimeSheetService
    {
        private readonly IGenericDBRepository<TimeSheet> _timeSheetRepository;

        public TimeSheetService(IGenericDBRepository<TimeSheet> timeSheetRepository)
        {
            _timeSheetRepository = timeSheetRepository;
        }

        public async Task<ServiceResponse<TimeSheet>> AddTimeSheetAsync(AddTimeSheetDTO addTimeSheetDto)
        {
            // Check if the timesheet already exists for the employee on the same date
            var existingTimeSheet = (await _timeSheetRepository.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = addTimeSheetDto.EmployeeId },
                new FilterDTO { PropertyName = "Date", Value = addTimeSheetDto.Date }
            })).FirstOrDefault();

            if (existingTimeSheet != null)
                return ServiceResponse<TimeSheet>.FailureResponse("Timesheet already exists for the given date.", 400);

            // Create a new timesheet entry
            var newTimeSheet = new TimeSheet
            {
                EmployeeId = addTimeSheetDto.EmployeeId,
                Date = addTimeSheetDto.Date,
                StartTime = addTimeSheetDto.StartTime,
                EndTime = addTimeSheetDto.EndTime,
                Description = addTimeSheetDto.Description
            };

            var success = await _timeSheetRepository.AddAsync(newTimeSheet);
            if (success)
                return ServiceResponse<TimeSheet>.SuccessResponse(newTimeSheet, "Timesheet added successfully.", 201);

            return ServiceResponse<TimeSheet>.FailureResponse("Failed to add timesheet.", 500);
        }

        public async Task<ServiceResponse<IEnumerable<TimeSheet>>> GetAllTimeSheetsOfEmployeeAsync(int employeeId)
        {
            var timeSheets = await _timeSheetRepository.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "EmployeeId", Value = employeeId }
            });

            if (!timeSheets.Any())
                return ServiceResponse<IEnumerable<TimeSheet>>.FailureResponse("No timesheets found for this employee.", 404);

            return ServiceResponse<IEnumerable<TimeSheet>>.SuccessResponse(timeSheets, "Timesheets fetched successfully.", 200);
        }

        public async Task<ServiceResponse<TimeSheet>> UpdateTimeSheetAsync(int id, UpdateTimeSheetDTO updateTimeSheetDto)
        {
            var timeSheet = await _timeSheetRepository.GetByIdAsync(id);
            if (timeSheet == null)
                return ServiceResponse<TimeSheet>.FailureResponse("Timesheet not found.", 404);

            // Update the timesheet details
            timeSheet.StartTime = updateTimeSheetDto.StartTime ?? timeSheet.StartTime;
            timeSheet.EndTime = updateTimeSheetDto.EndTime ?? timeSheet.EndTime;
            timeSheet.Description = updateTimeSheetDto.Description ?? timeSheet.Description;

            var success = await _timeSheetRepository.UpdateAsync(timeSheet);
            if (success)
                return ServiceResponse<TimeSheet>.SuccessResponse(timeSheet, "Timesheet updated successfully.", 200);

            return ServiceResponse<TimeSheet>.FailureResponse("Failed to update timesheet.", 500);
        }

        public async Task<ServiceResponse<bool>> DeleteTimeSheetAsync(int id)
        {
            var timeSheet = await _timeSheetRepository.GetByIdAsync(id);
            if (timeSheet == null)
                return ServiceResponse<bool>.FailureResponse("Timesheet not found.", 404);

            var success = await _timeSheetRepository.DeleteAsync(id);
            if (success)
                return ServiceResponse<bool>.SuccessResponse(true, "Timesheet deleted successfully.", 200);

            return ServiceResponse<bool>.FailureResponse("Failed to delete timesheet.", 500);
        }
    }
}
