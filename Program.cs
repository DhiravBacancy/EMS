using EMS.Data;
using EMS.Repositories;
using EMS.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using EMS.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IExportTimesheetsToExcelService, ExportTimesheetsToExcelService>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();


//builder.Services.AddScoped<IExportToPDFService, ExportToPDFService>();

builder.Services.AddScoped(typeof(IGenericDBRepository<>), typeof(GenericDBRepository<>));
builder.Services.AddScoped(typeof(IGenericDBService<>), typeof(GenericDBService<>));

builder.Services.AddMemoryCache(); // Register IMemoryCache
builder.Services.AddScoped<ICacheService, CacheService>();

// Register IAuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Add DbContext with SQL Server and logging
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()  // Optional: This enables logging of parameter values for debugging
           .LogTo(Console.WriteLine, LogLevel.Information) // Log SQL queries to the console
);


// Validate JWT settings before usage
var jwtIssuer = builder.Configuration["JwtSettings:Issuer"];
var jwtAudience = builder.Configuration["JwtSettings:Audience"];
var jwtSecretKey = builder.Configuration["JwtSettings:SecretKey"];

if (string.IsNullOrEmpty(jwtIssuer) || string.IsNullOrEmpty(jwtAudience) || string.IsNullOrEmpty(jwtSecretKey))
{
    throw new InvalidOperationException("JWT settings are missing in configuration.");
}

 

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ExceptionHandlingMiddleware>(); // Global error handling
app.UseMiddleware<ResponseWrapperMiddleware>();


app.UseHttpsRedirection();


//app.UseMiddleware<JwtAuthenticationMiddleware>();


app.UseRouting();       
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();
