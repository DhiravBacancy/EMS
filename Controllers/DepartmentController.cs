using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using EMS.DTOs;
using EMS.Models;
using EMS.Helpers;

namespace EMS.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IGenericDBService<Department> _departmentService;

        public DepartmentController(IGenericDBService<Department> departmentService)
        {
            _departmentService = departmentService;
        }

        // Create Department
        [HttpPost]
        public async Task<IActionResult> AddDepartment([FromBody] AddOrUpdateDepartmentDTO departmentDto)
        {
            if (departmentDto == null)
                return BadRequest(new { message = "Invalid department data." });

            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var existingDepartment = (await _departmentService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "DepartmentName", Value = departmentDto.DepartmentName }
            })).FirstOrDefault();

            if (existingDepartment != null)
                return Conflict(new { message = "Department with this name already exists." });

            var newDepartment = new Department { DepartmentName = departmentDto.DepartmentName };

            return await _departmentService.AddAsync(newDepartment)
                ? Ok(new { message = "Department added successfully." })
                : StatusCode(500, new { message = "Failed to add department." });
        }

        // Get All Departments
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllAsync();
            return departments.Any() ? Ok(departments) : NotFound(new { message = "No departments found." });
        }

        // Get Department by ID
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            return department != null ? Ok(department) : NotFound(new { message = "Department not found." });
        }

        // Update Department
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] AddOrUpdateDepartmentDTO departmentDto)
        {
            if (departmentDto == null)
                return BadRequest(new { message = "Invalid department data." });

            var validationResult = DTOValidationHelper.ValidateModelState(ModelState);
            if (validationResult != null) return validationResult;

            var existingDepartment = await _departmentService.GetByIdAsync(id);
            if (existingDepartment == null)
                return NotFound(new { message = "Department not found." });

            var duplicateDepartment = (await _departmentService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "DepartmentName", Value = departmentDto.DepartmentName }
            })).FirstOrDefault();

            if (duplicateDepartment != null && duplicateDepartment.DepartmentId != id)
                return Conflict(new { message = "Department name is already in use by another department." });

            existingDepartment.DepartmentName = departmentDto.DepartmentName;

            return await _departmentService.UpdateAsync(existingDepartment)
                ? Ok(new { message = "Department updated successfully." })
                : StatusCode(500, new { message = "Failed to update department." });
        }

        // Delete Department
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return NotFound(new { message = "Department not found." });

            // Assuming `_employeeService` should be used to check for employees in a department
            var hasEmployees = await _departmentService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "DepartmentId", Value = id }
            });

            if (!hasEmployees.Any())
                return BadRequest(new { message = "Department cannot be deleted because it contains employees." });

            return await _departmentService.DeleteAsync(id)
                ? Ok(new { message = "Department deleted successfully." })
                : StatusCode(500, new { message = "Failed to delete department." });
        }
    }
}
