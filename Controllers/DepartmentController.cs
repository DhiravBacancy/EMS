using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using EMS.DTOs;
using EMS.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            if (departmentDto == null || string.IsNullOrWhiteSpace(departmentDto.DepartmentName))
                return BadRequest(new { message = "Invalid input. Please provide valid department data." });

            // Check if department already exists
            var existingDepartment = (await _departmentService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "DepartmentName", Value = departmentDto.DepartmentName }
            })).FirstOrDefault();

            if (existingDepartment != null)
                return BadRequest(new { message = "Department with this name already exists." });

            // Add new department
            var newDepartment = new Department
            {
                DepartmentName = departmentDto.DepartmentName
            };

            if (await _departmentService.AddAsync(newDepartment))
                return Ok(new { message = "Department added successfully." });

            return StatusCode(500, new { message = "Failed to add department due to an internal server error." });
        }

        // Get All Departments
        [HttpGet]
        public async Task<IActionResult> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllAsync();

            if (departments == null || !departments.Any())
                return NotFound(new { message = "No departments found." });

            return Ok(departments);
        }

        // Get Department by ID
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDepartmentById(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return NotFound(new { message = "Department not found." });

            return Ok(department);
        }

        // Update Department
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDepartment(int id, [FromBody] AddOrUpdateDepartmentDTO departmentDto)
        {
            if (departmentDto == null || string.IsNullOrWhiteSpace(departmentDto.DepartmentName))
                return BadRequest(new { message = "Invalid input. Please provide valid department data." });

            var existingDepartment = await _departmentService.GetByIdAsync(id);
            if (existingDepartment == null)
                return NotFound(new { message = "Department not found." });

            // Check for name conflict when updating
            var departmentCheck = (await _departmentService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "DepartmentName", Value = departmentDto.DepartmentName }
            })).FirstOrDefault();

            if (departmentCheck != null && departmentCheck.DepartmentId != id)
                return BadRequest(new { message = "Department name is already in use by another department." });

            // Update department details
            existingDepartment.DepartmentName = departmentDto.DepartmentName;

            if (await _departmentService.UpdateAsync(existingDepartment))
                return Ok(new { message = "Department updated successfully." });

            return StatusCode(500, new { message = "Failed to update department due to an internal server error." });
        }

        // Delete Department
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDepartment(int id)
        {
            var department = await _departmentService.GetByIdAsync(id);
            if (department == null)
                return NotFound(new { message = "Department not found." });

            // Check if the department has employees before deleting
            var employees = await _departmentService.GetByMultipleConditionsAsync(new List<FilterDTO>
            {
                new FilterDTO { PropertyName = "DepartmentId", Value = id }
            });

            if (employees.Any())
                return BadRequest(new { message = "Department cannot be deleted because it contains employees." });

            if (await _departmentService.DeleteAsync(id))
                return Ok(new { message = "Department deleted successfully." });

            return StatusCode(500, new { message = "Failed to delete department due to an internal server error." });
        }
    }
}
