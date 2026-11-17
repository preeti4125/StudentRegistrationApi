using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StudentManagementApi.Authorization;
using StudentManagementApi.Data;
using StudentManagementApi.Middlewares;
using StudentManagementApi.Repositories;
using StudentManagementApi.Repositories.Interfaces;
using StudentManagementApi.Services;
using StudentManagementApi.Services.Interfaces;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add controllers and API explorer
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Configure Swagger with JWT Bearer Support
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer [space] and then your token'",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
            },
            Array.Empty<string>()
        }
    });
});

// Database Context (SQLite)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection") ?? "Data Source=students.db"));

// Dependency Injection Registrations
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ITokenService, TokenService>();

// JWT Authentication Setup
var jwtKey = builder.Configuration["Jwt:Key"] ?? "SuperSecretKeyLongEnoughForSigning123!";
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"] ?? "StudentApi",
            ValidAudience = builder.Configuration["Jwt:Audience"] ?? "StudentApiUsers",
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Custom Authorization Policy Setup
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.Requirements.Add(new AdminOnlyRequirement()));
});
builder.Services.AddSingleton<IAuthorizationHandler, AdminOnlyHandler>();

var app = builder.Build();

// 1. Custom Exception Handling Middleware Pipeline Registration
app.UseMiddleware<ExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// 2. Authentication & Authorization Middlewares
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();