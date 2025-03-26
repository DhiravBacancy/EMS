using EMS.DTOs;
using EMS.HelperClasses;
using EMS.Models;
using EMS.Repositories;

namespace EMS.Service
{
    public interface IDepartmentService
    {
        Task<ServiceResponse<Department>> AddDepartmentAsync(AddOrUpdateDepartmentDTO departmentDto);
        Task<ServiceResponse<IEnumerable<Department>>> GetAllDepartmentsAsync();
        Task<ServiceResponse<Department>> GetDepartmentByIdAsync(int id);
        Task<ServiceResponse<Department>> UpdateDepartmentAsync(int id, AddOrUpdateDepartmentDTO departmentDto);
    }

    public class DepartmentService : IDepartmentService
    {
        private readonly IGenericDBRepository<Department> _departmentService;

        public DepartmentService(IGenericDBRepository<Department> departmentService)
        {
            _departmentService = departmentService;
        }

        public async Task<ServiceResponse<Department>> AddDepartmentAsync(AddOrUpdateDepartmentDTO departmentDto)
        {
            // Check if department with the same name already exists
            var existingDepartment = (await _departmentService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "DepartmentName", Value = departmentDto.DepartmentName } }
            )).FirstOrDefault();

            if (existingDepartment != null)
                return ServiceResponse<Department>.FailureResponse("Department with this name already exists.", 409);

            var newDepartment = new Department
            {
                DepartmentName = departmentDto.DepartmentName
            };

            var success = await _departmentService.AddAsync(newDepartment);
            if (success)
                return ServiceResponse<Department>.SuccessResponse(newDepartment, "Department added successfully.", 201);

            return ServiceResponse<Department>.FailureResponse("Failed to add department.", 500);
        }

        public async Task<ServiceResponse<IEnumerable<Department>>> GetAllDepartmentsAsync()
        {
            var departments = await _departmentService.GetAllAsync();
            if (!departments.Any())
                return ServiceResponse<IEnumerable<Department>>.FailureResponse("No departments found.", 404);

            return ServiceResponse<IEnumerable<Department>>.SuccessResponse(departments, "Departments fetched successfully.", 200);
        }

        public async Task<ServiceResponse<Department>> GetDepartmentByIdAsync(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return ServiceResponse<Department>.FailureResponse("Department not found.", 404);

            return ServiceResponse<Department>.SuccessResponse(department, "Department fetched successfully.", 200);
        }

        public async Task<ServiceResponse<Department>> UpdateDepartmentAsync(int id, AddOrUpdateDepartmentDTO departmentDto)
        {
            var existingDepartment = await _departmentService.GetByIdAsync(id);
            if (existingDepartment == null)
                return ServiceResponse<Department>.FailureResponse("Department not found.", 404);

            var duplicateDepartment = (await _departmentService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "DepartmentName", Value = departmentDto.DepartmentName } }
            )).FirstOrDefault();

            if (duplicateDepartment != null && duplicateDepartment.DepartmentId != id)
                return ServiceResponse<Department>.FailureResponse("Department name is already in use by another department.", 409);

            existingDepartment.DepartmentName = departmentDto.DepartmentName;

            var success = await _departmentService.UpdateAsync(existingDepartment);
            if (success)
                return ServiceResponse<Department>.SuccessResponse(existingDepartment, "Department updated successfully.", 200);

            return ServiceResponse<Department>.FailureResponse("Failed to update department.", 500);
        }
    }
}
