using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Admin.Services;
using AIPlacement.Application.Certifications.Interfaces;
using AIPlacement.Application.Certifications.Services;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Company.Services;
using AIPlacement.Application.Placements.Interfaces;
using AIPlacement.Application.Placements.Services;
using AIPlacement.Application.Projects.Interfaces;
using AIPlacement.Application.Projects.Services;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Application.Resumes.Services;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.Application.Skills.Services;
using AIPlacement.Application.Students.Interfaces;
using AIPlacement.Application.Students.Services;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Infrastructure.Jobs;
using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Application.Recruitment.Services;
using AIPlacement.Infrastructure.Recruitment;
using AIPlacement.Infrastructure.Repositories;
using AIPlacement.Infrastructure.Certifications;
using AIPlacement.Infrastructure.Company;
using AIPlacement.Infrastructure.Data;
using AIPlacement.Infrastructure.Projects;
using AIPlacement.Infrastructure.Resumes;
using AIPlacement.Infrastructure.Skills;
using AIPlacement.Infrastructure.Students;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

using System.Text;

var builder = WebApplication.CreateBuilder(args);

// ===============================
// Controllers
// ===============================

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();

// ===============================
// Swagger + JWT Authorize Button
// ===============================

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter JWT token"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// ===============================
// JWT Authentication
// ===============================

var jwtKey = builder.Configuration["Jwt:Key"];

if (string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "JWT signing key is missing from configuration.");
}

builder.Services
    .AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme =
            JwtBearerDefaults.AuthenticationScheme;

        options.DefaultChallengeScheme =
            JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer =
                    builder.Configuration["Jwt:Issuer"],

                ValidAudience =
                    builder.Configuration["Jwt:Audience"],

                IssuerSigningKey =
                    new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtKey)),

                ClockSkew = TimeSpan.Zero
            };
    });

// ===============================
// Database
// ===============================

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString(
            "DefaultConnection")));

// ===============================
// Student
// ===============================

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

// ===============================
// Resume
// ===============================

builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IResumeRepository, ResumeRepository>();

// ===============================
// Company
// ===============================

builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// ===============================
// Job Drives
// ===============================

builder.Services.AddScoped<IJobDriveRepository, JobDriveRepository>();
builder.Services.AddScoped<IJobDriveService, JobDriveService>();
builder.Services.AddScoped<IEligibilityCriteriaRepository, EligibilityCriteriaRepository>();
builder.Services.AddScoped<IEligibilityCriteriaService, EligibilityCriteriaService>();
builder.Services.AddScoped<IJobEligibleBranchRepository, JobEligibleBranchRepository>();
builder.Services.AddScoped<IJobEligibleBranchService, JobEligibleBranchService>();

// ===============================
// Skills
// ===============================

builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();

// ===============================
// Certifications
// ===============================

builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();

// ===============================
// Projects
// ===============================

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// ===============================
// Admin + Analytics (Pair 3)
// ===============================

builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IUserRecordsService, UserRecordsService>();
builder.Services.AddScoped<IJobDriveApprovalService, JobDriveApprovalService>();
builder.Services.AddScoped<IApplicationMonitoringService, ApplicationMonitoringService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IPlacementService, PlacementService>();

// ===============================
// Recruitment
// ===============================

builder.Services.AddScoped<IRecruitmentRepository, RecruitmentRepository>();
builder.Services.AddScoped<IRecruitmentService, RecruitmentService>();

var app = builder.Build();

// ===============================
// HTTP Pipeline
// ===============================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// IMPORTANT: Authentication before Authorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
