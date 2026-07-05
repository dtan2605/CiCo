using System.Text;
using AutoMapper;
using cico.Infrastructure;
using cico.Infrastructure.Auth;
using cico.Infrastructure.Persistence;
using FluentValidation;
using Hangfire;
using Hangfire.Dashboard;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.CookiePolicy;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ================== CORS ==================
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins("http://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials());
});

// ================== Serilog ==================
builder.Host.UseSerilog((context, cfg) =>
    cfg.ReadFrom.Configuration(context.Configuration));

// ================== JWT Authentication ==================
var jwtSection = builder.Configuration.GetSection(JwtSettings.SectionName);
var jwtSettings = jwtSection.Get<JwtSettings>()
    ?? throw new InvalidOperationException("JwtSettings not configured");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidAudience = jwtSettings.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("Admin"));

    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireRole("Manager"));

    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("Employee"));

    options.AddPolicy("ManagerOrAdmin", policy =>
        policy.RequireRole("Admin", "Manager"));
});

// ================== Rate Limiting ==================
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("EmployeePolicy", opt =>
    {
        opt.PermitLimit = 100;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 10;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });

    options.AddFixedWindowLimiter("WritePolicy", opt =>
    {
        opt.PermitLimit = 20;
        opt.Window = TimeSpan.FromMinutes(1);
    });

    options.AddFixedWindowLimiter("DeletePolicy", opt =>
    {
        opt.PermitLimit = 10;
        opt.Window = TimeSpan.FromMinutes(1);
    });

    options.AddFixedWindowLimiter("LoginPolicy", opt =>
    {
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromMinutes(1);
        opt.QueueLimit = 0;
    });

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// ================== Cookie Policy ==================
builder.Services.Configure<CookiePolicyOptions>(options =>
{
    options.HttpOnly = HttpOnlyPolicy.Always;
    options.MinimumSameSitePolicy = SameSiteMode.Strict;
    options.Secure = CookieSecurePolicy.Always;
});

// ================== Database ==================
builder.Services.AddDbContext<CICODbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ================== FluentValidation ==================
builder.Services.AddValidatorsFromAssembly(
    typeof(cico.Application.AssemblyReference).Assembly);

// ================== Hangfire ==================
builder.Services.AddHangfire(config =>
    config.UseSqlServerStorage(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer();

// ================== SignalR ==================
builder.Services.AddSignalR();

// ================== OpenAPI ==================
builder.Services.AddOpenApi();

// ================== Controllers ==================
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(
            new System.Text.Json.Serialization.JsonStringEnumConverter());
    });

// ================== Application + Infrastructure ==================
builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(
        typeof(cico.Application.AssemblyReference).Assembly));

builder.Services.AddAutoMapper(cfg =>
    cfg.AddMaps(typeof(cico.Application.AssemblyReference).Assembly));

var app = builder.Build();

// ================== Middleware Pipeline ==================
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    app.UseExceptionHandler(appBuilder =>
        appBuilder.Run(async context =>
        {
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(new { error = "An internal error occurred" });
        }));

    app.UseHsts();
}

// Security headers
app.Use(async (context, next) =>
{
    context.Response.Headers.Append("X-Content-Type-Options", "nosniff");
    context.Response.Headers.Append("X-Frame-Options", "DENY");
    context.Response.Headers.Append("Referrer-Policy", "no-referrer");
    context.Response.Headers.Append("X-XSS-Protection", "1; mode=block");
    await next();
});

app.UseCors();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseSerilogRequestLogging();
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();
app.UseCookiePolicy();
app.UseHangfireDashboard("/hangfire", new DashboardOptions
{
    Authorization =
    [
        new HangfireAuthorizationFilter()
    ]
});

// ================== Recurring Jobs ==================
using (var scope = app.Services.CreateScope())
{
    var manager = scope.ServiceProvider
        .GetRequiredService<IRecurringJobManager>();

    manager.AddOrUpdate<
        cico.Infrastructure.Jobs.AttendanceReminderJob>(
        "daily-attendance-reminder",
        job => job.RunAsync(),
        "0 8 * * *"); // 8:00 AM daily

    manager.AddOrUpdate<
        cico.Infrastructure.Jobs.NotificationCleanupJob>(
        "notification-cleanup",
        job => job.RunAsync(),
        "0 2 * * 0"); // 2:00 AM every Sunday

    manager.AddOrUpdate<
        cico.Infrastructure.Jobs.AttendanceSyncJob>(
        "attendance-sync",
        job => job.RunAsync(),
        "*/5 * * * *"); // every 5 minutes

    manager.AddOrUpdate<
        cico.Infrastructure.Jobs.DeviceHealthCheckJob>(
        "device-health-check",
        job => job.RunAsync(),
        "*/1 * * * *"); // every 1 minute
}

app.MapControllers();

// ================== Database Seed ==================
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider
        .GetRequiredService<CICODbContext>();
    await DbInitializer.InitializeAsync(context);
}

// ================== SignalR Hubs ==================
app.MapHub<
    cico.Infrastructure.SignalR.Hubs.DashboardHub>(
    "/hub/dashboard");
app.MapHub<
    cico.Infrastructure.SignalR.Hubs.NotificationHub>(
    "/hub/notifications");

app.Run();

class HangfireAuthorizationFilter : Hangfire.Dashboard.IDashboardAuthorizationFilter
{
    public bool Authorize(Hangfire.Dashboard.DashboardContext context)
    {
        var httpContext = context.GetHttpContext();
        return httpContext.User.Identity?.IsAuthenticated == true
            && httpContext.User.IsInRole("Admin");
    }
}
