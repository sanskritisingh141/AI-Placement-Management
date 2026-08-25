using AIPlacement.Infrastructure.Company;
using AIPlacement.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Company.Services;
using AIPlacement.Application.Students.Interfaces;
using AIPlacement.Application.Students.Services;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Application.Resumes.Services;
using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Admin.Services;
using AIPlacement.Application.Placements.Interfaces;
using AIPlacement.Application.Placements.Services;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Infrastructure.Jobs;
using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Application.Recruitment.Services;
using AIPlacement.Infrastructure.Recruitment;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<IJobDriveRepository, JobDriveRepository>();
builder.Services.AddScoped<IJobDriveService, JobDriveService>();

// Admin + Analytics (Pair 3)
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IUserRecordsService, UserRecordsService>();
builder.Services.AddScoped<IJobDriveApprovalService, JobDriveApprovalService>();
builder.Services.AddScoped<IApplicationMonitoringService, ApplicationMonitoringService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IPlacementService, PlacementService>();
builder.Services.AddScoped<IRecruitmentRepository, RecruitmentRepository>();
builder.Services.AddScoped<IRecruitmentService, RecruitmentService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
