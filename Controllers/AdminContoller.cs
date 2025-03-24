using EMS.Service;
using Microsoft.AspNetCore.Mvc;
using EMS.DTOs;
using EMS.Models;
using EMS.Helpers;


namespace EMS.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IGenericDBService<Admin> _adminService;

        public AdminController(IGenericDBService<Admin> adminService)
        {
            _adminService = adminService;
        }

        [HttpPost("Add")]
        public async Task<IActionResult> AddAdmin([FromBody] AddAdminDTO addAdminDto)
        {
            DTOValidationHelper.ValidateModelState(ModelState); // Throws validation error if invalid

            var existingAdmin = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = addAdminDto.Email } }
            )).FirstOrDefault();

            if (existingAdmin != null)
                throw new CustomException("Email is already in use", 400);

            var newAdmin = new Admin
            {
                FirstName = addAdminDto.FirstName,
                LastName = addAdminDto.LastName,
                Passsword = BCrypt.Net.BCrypt.HashPassword(addAdminDto.Passsword),
                Email = addAdminDto.Email,
                Phone = addAdminDto.Phone,
            };

            await _adminService.AddAsync(newAdmin);
            return Ok("Admin added successfully"); // ResponseWrapper will wrap this
        }

        [HttpGet("All")]
        public async Task<IActionResult> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAsync();
            if (admins == null || !admins.Any())
                throw new CustomException("No Admin Found", 404);

            return Ok(admins);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAdminById(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                throw new CustomException("Admin not found", 404);

            return Ok(admin);
        }

        [HttpPut("Update/{id}")]
        public async Task<IActionResult> UpdateAdmin(int id, [FromBody] UpdateAdminDTO updateAdminDto)
        {
            DTOValidationHelper.ValidateModelState(ModelState);

            var existingAdmin = await _adminService.GetByIdAsync(id);
            if (existingAdmin == null)
                throw new CustomException("Admin not found", 404);

            var emailCheck = (await _adminService.GetByMultipleConditionsAsync(
                new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = updateAdminDto.Email } }
            )).FirstOrDefault();

            if (emailCheck != null && emailCheck.AdminId != id)
                throw new CustomException("Email is already in use by another admin", 400);

            existingAdmin.FirstName = updateAdminDto.FirstName ?? existingAdmin.FirstName;
            existingAdmin.LastName = updateAdminDto.LastName ?? existingAdmin.LastName;
            existingAdmin.Email = updateAdminDto.Email ?? existingAdmin.Email;
            existingAdmin.Phone = updateAdminDto.Phone ?? existingAdmin.Phone;
            existingAdmin.UpdatedAt = DateTime.UtcNow;

            await _adminService.UpdateAsync(existingAdmin);
            return Ok("Admin updated successfully");
        }

        [HttpDelete("Delete/{id}")]
        public async Task<IActionResult> DeleteAdmin(int id)
        {
            var admin = await _adminService.GetByIdAsync(id);
            if (admin == null)
                throw new CustomException("Admin not found", 404);

            await _adminService.DeleteAsync(admin.AdminId);
            return Ok("Admin deleted successfully");
        }
    }
}
