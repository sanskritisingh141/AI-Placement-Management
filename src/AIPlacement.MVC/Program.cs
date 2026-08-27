
using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Admin.Services;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Company.Services;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.Application.Skills.Services;
using AIPlacement.Infrastructure.Company;
using AIPlacement.Infrastructure.Jobs;
using AIPlacement.Infrastructure.Repositories;
using AIPlacement.Infrastructure.Skills;
using AllPlacement.MVC.Data;
using Microsoft.EntityFrameworkCore;

using AIPlacement.Infrastructure.Data;
using AIPlacement.Application.Certifications.Interfaces;
using AIPlacement.Application.Certifications.Services;
using AIPlacement.Application.Projects.Interfaces;
using AIPlacement.Application.Projects.Services;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Application.Resumes.Services;
using AIPlacement.Application.Students.Interfaces;
using AIPlacement.Application.Students.Services;

using AIPlacement.Infrastructure.Certifications;
using AIPlacement.Infrastructure.Projects;
using AIPlacement.Infrastructure.Resumes;
using AIPlacement.Infrastructure.Students;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<
    AIPlacement.Infrastructure.Data.ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Pair 2: Company and Job modules
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddScoped<ICompanyService, CompanyService>();
builder.Services.AddScoped<IJobDriveRepository, JobDriveRepository>();
builder.Services.AddScoped<IJobDriveService, JobDriveService>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();
builder.Services.AddScoped<ISkillService, SkillService>();

builder.Services.AddControllersWithViews();

// Admin login state is tracked with a lightweight session cookie until the shared
// Identity/role-based auth (TSK-05) is wired up across the whole app.
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Student module
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IStudentRepository, StudentRepository>();

builder.Services.AddScoped<ISkillService, SkillService>();
builder.Services.AddScoped<ISkillRepository, SkillRepository>();

builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();

builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IResumeRepository, ResumeRepository>();

// Admin + Analytics
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IUserRecordsService, UserRecordsService>();
builder.Services.AddScoped<IJobDriveApprovalService, JobDriveApprovalService>();
builder.Services.AddScoped<IApplicationMonitoringService, ApplicationMonitoringService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();

// EligibilityCriteria and JobEligibleBranches (Pair 2 Member 3)
builder.Services.AddScoped<IEligibilityCriteriaService, EligibilityCriteriaService>();
builder.Services.AddScoped<IJobEligibleBranchService, JobEligibleBranchService>();
builder.Services.AddScoped<IEligibilityCriteriaRepository, EligibilityCriteriaRepository>();
builder.Services.AddScoped<IJobEligibleBranchRepository, JobEligibleBranchRepository>();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
