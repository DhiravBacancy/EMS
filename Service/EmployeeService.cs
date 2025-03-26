using EMS.DTOs;
using EMS.HelperClasses;
using EMS.Models;
using EMS.Repositories;
using EMS.Service.EMS.Service;

namespace EMS.Service
{
    namespace EMS.Service
    {
        public interface IEmployeeService
        {
            Task<ServiceResponse<Employee>> AddEmployeeAsync(AddEmployeeDTO employeeDto);
            Task<ServiceResponse<IEnumerable<Employee>>> GetAllEmployeesAsync();
            Task<ServiceResponse<Employee>> GetEmployeeByIdAsync(int id);
            Task<ServiceResponse<Employee>> UpdateEmployeeAsync(int id, UdpateEmployeeDTO updateEmployeeDto);
            Task<ServiceResponse<bool>> DeleteEmployeeAsync(int id);
        }
    }

    public class EmployeeService : IEmployeeService
    {
        private readonly IGenericDBRepository<Employee> _employeeService;

        public EmployeeService(IGenericDBRepository<Employee> employeeService)
        {
            _employeeService = employeeService;
        }

        public async Task<ServiceResponse<Employee>> AddEmployeeAsync(AddEmployeeDTO employeeDto)
        {
            var existingEmployee = (await _employeeService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = employeeDto.Email } }
            )).FirstOrDefault();

            if (existingEmployee != null)
                return ServiceResponse<Employee>.FailureResponse("Email is already in use", 400);

            var newEmployee = new Employee
            {
                FirstName = employeeDto.FirstName,
                LastName = employeeDto.LastName,
                Email = employeeDto.Email,
                Password = BCrypt.Net.BCrypt.HashPassword(employeeDto.Password),
                Phone = employeeDto.Phone,
                DateOfBirth = employeeDto.DateOfBirth,
                Address = employeeDto.Address,
                TechStack = employeeDto.TechStack
            };

            var success = await _employeeService.AddAsync(newEmployee);
            if (success)
                return ServiceResponse<Employee>.SuccessResponse(newEmployee, "Employee added successfully.", 201);

            return ServiceResponse<Employee>.FailureResponse("Failed to add Employee", 500);
        }

        public async Task<ServiceResponse<IEnumerable<Employee>>> GetAllEmployeesAsync()
        {
            var employees = await _employeeService.GetAllAsync();
            if (!employees.Any())
                return ServiceResponse<IEnumerable<Employee>>.FailureResponse("No employees found.", 404);

            return ServiceResponse<IEnumerable<Employee>>.SuccessResponse(employees, "Employees fetched successfully.", 200);
        }

        public async Task<ServiceResponse<Employee>> GetEmployeeByIdAsync(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return ServiceResponse<Employee>.FailureResponse("Employee not found.", 404);

            return ServiceResponse<Employee>.SuccessResponse(employee, "Employee fetched successfully.", 200);
        }

        public async Task<ServiceResponse<Employee>> UpdateEmployeeAsync(int id, UdpateEmployeeDTO updateEmployeeDto)
        {
            var existingEmployee = await _employeeService.GetByIdAsync(id);
            if (existingEmployee == null)
                return ServiceResponse<Employee>.FailureResponse("Employee not found.", 404);

            if (!string.IsNullOrEmpty(updateEmployeeDto.Email))
            {
                var emailCheck = (await _employeeService.GetByMultipleConditionsAsync(
                    new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = updateEmployeeDto.Email } }
                )).FirstOrDefault();

                if (emailCheck != null && emailCheck.EmployeeId != id)
                    return ServiceResponse<Employee>.FailureResponse("Email is already in use by another employee.", 400);
            }

            existingEmployee.FirstName = updateEmployeeDto.FirstName ?? existingEmployee.FirstName;
            existingEmployee.LastName = updateEmployeeDto.LastName ?? existingEmployee.LastName;
            existingEmployee.Email = updateEmployeeDto.Email ?? existingEmployee.Email;
            existingEmployee.Phone = updateEmployeeDto.Phone ?? existingEmployee.Phone;
            existingEmployee.DateOfBirth = updateEmployeeDto.DateOfBirth ?? existingEmployee.DateOfBirth;
            existingEmployee.Address = updateEmployeeDto.Address ?? existingEmployee.Address;
            existingEmployee.TechStack = updateEmployeeDto.TechStack ?? existingEmployee.TechStack;
            existingEmployee.UpdatedAt = DateTime.UtcNow;

            var success = await _employeeService.UpdateAsync(existingEmployee);
            if (success)
                return ServiceResponse<Employee>.SuccessResponse(existingEmployee, "Employee updated successfully.", 200);

            return ServiceResponse<Employee>.FailureResponse("Failed to update Employee.", 500);
        }

        public async Task<ServiceResponse<bool>> DeleteEmployeeAsync(int id)
        {
            var employee = await _employeeService.GetByIdAsync(id);
            if (employee == null)
                return ServiceResponse<bool>.FailureResponse("Employee not found.", 404);

            var success = await _employeeService.DeleteAsync(id);
            if (success)
                return ServiceResponse<bool>.SuccessResponse(true, "Employee deleted successfully.", 200);

            return ServiceResponse<bool>.FailureResponse("Failed to delete Employee.", 500);
        }
    }
}
