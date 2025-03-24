using EMS.DTOs;
using EMS.Models;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace EMS.Service
{
    public interface IAuthService
    {
        Task<TokenResponseDTO> LoginAsync(LoginDTO loginDto);
        void Logout(string token);
        bool IsTokenRevoked(string token);
        Task<IActionResult> PasswordReset(ResetPasswordDTO resetPasswordDto);
        Task<IActionResult> ResetPasswordWhenNotLoggedIn(string email);
        Task<bool> VerifyOTP(string email, string enteredOtp);
    }

    public class AuthService : IAuthService
    {
        private readonly IGenericDBService<Admin> _adminService;
        private readonly IGenericDBService<Employee> _employeeService;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IEmailService _emailService;

        public AuthService(IGenericDBService<Admin> adminService, IGenericDBService<Employee> employeeService, IConfiguration configuration, ICacheService cacheService, IEmailService emailService)
        {
            _adminService = adminService;
            _employeeService = employeeService;
            _configuration = configuration;
            _cacheService = cacheService;
            _emailService = emailService;
        }

        public async Task<TokenResponseDTO> LoginAsync(LoginDTO loginDto)
        {
            var filters = new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = loginDto.Email } };

            var admin = (await _adminService.GetByMultipleConditionsAsync(filters)).FirstOrDefault();
            GenerateTokenDTO generateTokenDto;

            if (admin != null && VerifyPassword(loginDto.Password, admin.Passsword))
            {
                generateTokenDto = new GenerateTokenDTO
                {
                    Id = admin.AdminId.ToString(),
                    Email = admin.Email,
                    Role = "Admin"
                };
            }
            else
            {
                var employee = (await _employeeService.GetByMultipleConditionsAsync(filters)).FirstOrDefault();
                if (employee != null && VerifyPassword(loginDto.Password, employee.Password))
                {
                    generateTokenDto = new GenerateTokenDTO
                    {
                        Id = employee.EmployeeId.ToString(),
                        Email = employee.Email,
                        Role = "Employee"  // ✅ Fixed role assignment
                    };
                }
                else
                {
                    return null; // Return null to indicate invalid credentials
                }
            }

            var token = GenerateToken(generateTokenDto);
            return new TokenResponseDTO { Token = token };
        }

        public async Task<IActionResult> PasswordReset(ResetPasswordDTO resetPasswordDto)
        {
            var filters = new List<FilterDTO> { new FilterDTO { PropertyName = "Email", Value = resetPasswordDto.Email } };

            var admin = (await _adminService.GetByMultipleConditionsAsync(filters)).FirstOrDefault();
            bool isUpdated = false;

            if (admin != null)
            {
                admin.Passsword = HashPassword(resetPasswordDto.NewPassword);
                isUpdated = await _adminService.UpdateAsync(admin);
            }
            else
            {
                var employee = (await _employeeService.GetByMultipleConditionsAsync(filters)).FirstOrDefault();
                if (employee != null)
                {
                    employee.Password = HashPassword(resetPasswordDto.NewPassword);
                    isUpdated = await _employeeService.UpdateAsync(employee);
                }
                else
                {
                    return new BadRequestObjectResult(new { message = "User not found" });
                }
            }

            if (isUpdated)
                return new OkObjectResult(new { message = "Password reset successfully" });
            else
                return new StatusCodeResult(500);
        }

        public async Task<IActionResult> ResetPasswordWhenNotLoggedIn(string email)
        {
            string otp = GenerateUniqueOTP();
            _cacheService.Set(email, otp, TimeSpan.FromMinutes(5));

            var emailMessage = new EmailMessageDTO
            {
                ToEmail = email,
                Subject = "Password Reset OTP",
                Body = $"<h1>Your OTP for password reset is: <b>{otp}</b></h1><p>It is valid for 5 minutes.</p>",
                IsHtml = true
            };

            await _emailService.SendEmailAsync(emailMessage);
            return new OkObjectResult(new { message = "OTP sent to your email successfully." });
        }

        public async Task<bool> VerifyOTP(string email, string enteredOtp)
        {
            var storedOtp = _cacheService.Get(email) as string;

            if (storedOtp != null && storedOtp == enteredOtp)
            {
                _cacheService.Remove(email);
                return true;
            }
            else
            {
                return false;
            }
        }

        private string GenerateUniqueOTP()
        {
            using (var rng = RandomNumberGenerator.Create())
            {
                byte[] bytes = new byte[4];
                rng.GetBytes(bytes);
                int otp = BitConverter.ToInt32(bytes, 0) & 999999;
                return otp.ToString("D6");
            }
        }

        public bool VerifyPassword(string password, string hashedPassword)
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private string GenerateToken(GenerateTokenDTO generateTokenDto)
        {
            var secretKey = _configuration["JwtSettings:SecretKey"];
            var issuer = _configuration["JwtSettings:Issuer"];
            var audience = _configuration["JwtSettings:Audience"];
            var expirationTime = DateTime.UtcNow.AddMinutes(30);

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, generateTokenDto.Id.ToString()),
                new Claim(ClaimTypes.Name, generateTokenDto.Email),
                new Claim(ClaimTypes.Role, generateTokenDto.Role),
                new Claim(JwtRegisteredClaimNames.Exp, new DateTimeOffset(expirationTime).ToUnixTimeSeconds().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expirationTime,
                Issuer = issuer,
                Audience = audience,
                SigningCredentials = credentials
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public void Logout(string token)
        {
            _cacheService.Set(token, "invalid", TimeSpan.FromMinutes(30));
        }

        public bool IsTokenRevoked(string token)
        {
            return _cacheService.Contains(token);
        }
    }
}
