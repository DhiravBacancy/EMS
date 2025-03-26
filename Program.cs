using EMS.Data;
using EMS.Repositories;
using EMS.Service;
using Microsoft.EntityFrameworkCore;
using EMS.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IEmailService, EmailService>();
builder.Services.AddScoped<IPdfService, PdfService>();
builder.Services.AddScoped<IExportTimesheetsToExcelService, ExportTimesheetsToExcelService>();
//builder.Services.AddScoped<IEmployeeService, EmployeeService>();


//builder.Services.AddScoped<IExportToPDFService, ExportToPDFService>();

builder.Services.AddScoped(typeof(IGenericDBRepository<>), typeof(GenericDBRepository<>));
builder.Services.AddScoped(typeof(IGenericDBService<>), typeof(GenericDBService<>));

builder.Services.AddMemoryCache(); // Register IMemoryCache
builder.Services.AddScoped<ICacheService, CacheService>();
builder.Services.AddScoped<IAdminService, AdminService>();

// Register IAuthService
builder.Services.AddScoped<IAuthService, AuthService>();

// Add DbContext with SQL Server and logging
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
           .EnableSensitiveDataLogging()  // Optional: This enables logging of parameter values for debugging
           .LogTo(Console.WriteLine, LogLevel.Information) // Log SQL queries to the console
);


// Add authentication using JWT Bearer Tokens
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"])),
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

                // Check the success of the response, and fail if the token is revoked
                if (response.Success && response.Data)
                {
                    context.Fail("This token has been revoked.");
                }
            }
        };


    });

builder.Services.AddControllers();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseMiddleware<ExceptionHandlingMiddleware>(); // Global error handling
app.UseMiddleware<ResponseWrapperMiddleware>();


app.UseHttpsRedirection();



app.UseStaticFiles(); // Enables serving files from wwwroot

app.UseRouting();       
app.UseAuthentication();

app.UseAuthorization();
app.MapControllers();

app.Run();
