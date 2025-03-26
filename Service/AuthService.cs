using EMS.DTOs;
using EMS.HelperClasses;
using EMS.Models;
using EMS.Repositories;
using Microsoft.AspNetCore.Http;
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
        Task<ServiceResponse<TokenResponseDTO>> LoginAsync(LoginDTO loginDto);
        Task<ServiceResponse<bool>> LogoutAsync(string token);
        Task<ServiceResponse<bool>> IsTokenRevokedAsync(string token);
        Task<ServiceResponse<bool>> PasswordResetAsync(ResetPasswordDTO resetPasswordDto);
        Task<ServiceResponse<bool>> ResetPasswordWhenNotLoggedInAsync(string email);
        Task<ServiceResponse<bool>> VerifyOTPAsync(string email, string enteredOtp);
    }

    public class AuthService : IAuthService
    {
        private readonly IGenericDBRepository<Admin> _adminService;
        private readonly IGenericDBRepository<Employee> _employeeService;
        private readonly IConfiguration _configuration;
        private readonly ICacheService _cacheService;
        private readonly IEmailService _emailService;

        public AuthService(IGenericDBRepository<Admin> adminService, IGenericDBRepository<Employee> employeeService, IConfiguration configuration, ICacheService cacheService, IEmailService emailService)
        {
            _adminService = adminService;
            _employeeService = employeeService;
            _configuration = configuration;
            _cacheService = cacheService;
            _emailService = emailService;
        }

        public async Task<ServiceResponse<TokenResponseDTO>> LoginAsync(LoginDTO loginDto)
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
                        Role = "Employee"
                    };
                }
                else
                {
                    return ServiceResponse<TokenResponseDTO>.FailureResponse("Invalid credentials", 401);
                }
            }

            var token = GenerateToken(generateTokenDto);
            return ServiceResponse<TokenResponseDTO>.SuccessResponse(new TokenResponseDTO { Token = token }, "Login successful", 200);
        }

        public async Task<ServiceResponse<bool>> PasswordResetAsync(ResetPasswordDTO resetPasswordDto)
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
                    return ServiceResponse<bool>.FailureResponse("User not found", 404);
                }
            }

            if (isUpdated)
                return ServiceResponse<bool>.SuccessResponse(true, "Password reset successfully", 200);

            return ServiceResponse<bool>.FailureResponse("Failed to reset password", 500);
        }

        public async Task<ServiceResponse<bool>> ResetPasswordWhenNotLoggedInAsync(string email)
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
            return ServiceResponse<bool>.SuccessResponse(true, "OTP sent to your email successfully.", 200);
        }

        public async Task<ServiceResponse<bool>> VerifyOTPAsync(string email, string enteredOtp)
        {
            var storedOtp = _cacheService.Get(email) as string;

            if (storedOtp != null && storedOtp == enteredOtp)
            {
                _cacheService.Remove(email);
                return ServiceResponse<bool>.SuccessResponse(true, "OTP verified successfully", 200);
            }

            return ServiceResponse<bool>.FailureResponse("Invalid OTP", 400);
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
            var expirationTime = DateTime.UtcNow.AddMinutes(120);

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

        public async Task<ServiceResponse<bool>> LogoutAsync(string token)
        {
            _cacheService.Set(token, "invalid", TimeSpan.FromMinutes(120));
            return ServiceResponse<bool>.SuccessResponse(true, "Logged out successfully", 200);
        }

        public async Task<ServiceResponse<bool>> IsTokenRevokedAsync(string token)
        {
            bool isRevoked = _cacheService.Contains(token);
            return ServiceResponse<bool>.SuccessResponse(isRevoked, "Token status checked", 200);
        }
    }
}
