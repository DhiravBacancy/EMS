using EMS.Data;
using EMS.Repositories;
using EMS.Service;
using Microsoft.EntityFrameworkCore;
using EMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EMS.Service.EMS.Service;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IExportTimesheetsToExcelService, ExportTimesheetsToExcelService>();
builder.Services.AddScoped(typeof(IGenericDBRepository<>), typeof(GenericDBRepository<>));

builder.Services.AddMemoryCache();
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
builder.Services.AddScoped<IDepartmentService, DepartmentService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<ILeaveService, LeaveService>();
builder.Services.AddScoped<ITimeSheetService, TimeSheetService>();
builder.Services.AddScoped<IAuthService, AuthService>();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()
           .LogTo(Console.WriteLine, LogLevel.Information)
);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"] ?? throw new InvalidOperationException("JWT Secret Key is not configured."))),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };

        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var authService = context.HttpContext.RequestServices.GetRequiredService<IAuthService>();
                var token = context.Request.Headers["Authorization"].ToString().Split(" ")[1];
                var response = await authService.IsTokenRevokedAsync(token);

                if (response.Success && response.Data)
                {
                    context.Fail("This token has been revoked.");
                }
            }
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
