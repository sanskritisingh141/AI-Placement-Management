using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Admin.Services;
using AIPlacement.Application.AI.Interfaces;
using AIPlacement.Application.AI.Services;
using AIPlacement.Application.Authentication;
using AIPlacement.Application.Authentication.Interfaces;
using AIPlacement.Application.Authentication.Services;
using AIPlacement.Application.Company.Interfaces;
using AIPlacement.Application.Company.Services;
using AIPlacement.Application.Jobs.Interfaces;
using AIPlacement.Application.Jobs.Services;
using AIPlacement.Application.Skills.Interfaces;
using AIPlacement.Application.Skills.Services;
using AIPlacement.Application.Students.Interfaces;
using AIPlacement.Application.Students.Services;
using AIPlacement.Application.Projects.Interfaces;
using AIPlacement.Application.Projects.Services;
using AIPlacement.Application.Certifications.Interfaces;
using AIPlacement.Application.Certifications.Services;
using AIPlacement.Application.Resumes.Interfaces;
using AIPlacement.Application.Resumes.Services;
using AIPlacement.Application.Recruitment.Interfaces;
using AIPlacement.Application.Recruitment.Services;
using AIPlacement.Infrastructure.Company;
using AIPlacement.Infrastructure.Jobs;
using AIPlacement.Infrastructure.Repositories;
using AIPlacement.Infrastructure.Skills;
using AIPlacement.Infrastructure.Authentication;
using AIPlacement.Infrastructure.Admin;
using AIPlacement.Infrastructure.AI;
using AIPlacement.Infrastructure.Students;
using AIPlacement.Infrastructure.Projects;
using AIPlacement.Infrastructure.Certifications;
using AIPlacement.Infrastructure.Resumes;
using AIPlacement.Infrastructure.Recruitment;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

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
builder.Services.AddScoped<IStudentRepository, StudentRepository>();
builder.Services.AddScoped<IStudentService, StudentService>();
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();
builder.Services.AddScoped<IProjectService, ProjectService>();
builder.Services.AddScoped<ICertificationRepository, CertificationRepository>();
builder.Services.AddScoped<ICertificationService, CertificationService>();
builder.Services.AddScoped<IResumeRepository, ResumeRepository>();
builder.Services.AddScoped<IResumeService, ResumeService>();
builder.Services.AddScoped<IRecruitmentRepository, RecruitmentRepository>();
builder.Services.AddScoped<IRecruitmentService, RecruitmentService>();
builder.Services.AddScoped<IAIRepository, AIRepository>();
builder.Services.AddScoped<IAIService, AIService>();
builder.Services.AddHttpClient<IAIAnalysisClient, FastApiAnalysisClient>(client =>
{
    var baseUrl = builder.Configuration["AIService:BaseUrl"] ?? "http://localhost:8000/";
    client.BaseAddress = new Uri(baseUrl.TrimEnd('/') + "/");
    client.Timeout = TimeSpan.FromSeconds(60);
});

builder.Services.AddControllersWithViews();

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.Cookie.Name = "AIPlacement.Auth";
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("StudentOnly", policy => policy.RequireRole(RoleNames.Student));
    options.AddPolicy("CompanyOnly", policy => policy.RequireRole(RoleNames.Company));
    options.AddPolicy("AdminOnly", policy => policy.RequireRole(RoleNames.Admin));
    options.AddPolicy("CompanyOrAdmin", policy =>
        policy.RequireRole(RoleNames.Company, RoleNames.Admin));
});

builder.Services.AddScoped<IAuthRepository, AuthRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddSingleton<IPasswordHashService, Pbkdf2PasswordHashService>();
builder.Services.AddScoped<DatabaseIdentitySeeder>();

// Admin + Analytics (Pair 3)
builder.Services.AddScoped<IAdminRepository, AdminRepository>();
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

using (var scope = app.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseIdentitySeeder>();
    await seeder.SeedAsync(
        builder.Configuration["SeedAdmin:Email"],
        builder.Configuration["SeedAdmin:Password"]);
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
