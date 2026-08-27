using AIPlacement.Application.Admin.Interfaces;
using AIPlacement.Application.Admin.Services;
using AIPlacement.Application.Jobs.Services;
using AllPlacement.MVC.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Add services to the container.
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

// Admin + Analytics (Pair 3)
builder.Services.AddScoped<IAdminAuthService, AdminAuthService>();
builder.Services.AddScoped<IUserRecordsService, UserRecordsService>();
builder.Services.AddScoped<IJobDriveApprovalService, JobDriveApprovalService>();
builder.Services.AddScoped<IApplicationMonitoringService, ApplicationMonitoringService>();
builder.Services.AddScoped<IAnalyticsService, AnalyticsService>();
builder.Services.AddScoped<IEligibilityCriteriaService, EligibilityCriteriaService>();
builder.Services.AddScoped<IJobEligibleBranchService, JobEligibleBranchService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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
