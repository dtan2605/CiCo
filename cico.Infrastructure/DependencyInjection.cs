using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Infrastructure.Repositories;
using cico.Infrastructure.Jobs;
using cico.Infrastructure.Services;
using cico.Infrastructure.SignalR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using cico.Infrastructure.Persistence;
using cico.Application.Interfaces;
using cico.Infrastructure.Auth;

namespace cico.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection
        AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
    {
        services.AddDbContext<CICODbContext>(
            options =>
                options.UseSqlServer(
                    configuration.GetConnectionString(
                        "DefaultConnection")));

        services.Configure<JwtSettings>(
            configuration.GetSection(
                JwtSettings.SectionName));

        // Repositories
        services.AddScoped<
            IEmployeeRepository,
            EmployeeRepository>();

        services.AddScoped<
            IAttendanceRepository,
            AttendanceRepository>();

        services.AddScoped<
            IDepartmentRepository,
            DepartmentRepository>();

        services.AddScoped<
            IPositionRepository,
            PositionRepository>();

        services.AddScoped<
            IDeviceRepository,
            DeviceRepository>();

        services.AddScoped<
            IReportRepository,
            ReportRepository>();

        services.AddScoped<
            IRefreshTokenRepository,
            RefreshTokenRepository>();

        services.AddScoped<
            IUserRepository,
            UserRepository>();

        services.AddScoped<
            IPermissionRepository,
            PermissionRepository>();

        services.AddScoped<
            INotificationRepository,
            NotificationRepository>();

        services.AddScoped<
            IScheduleRepository,
            ScheduleRepository>();

        services.AddScoped<
            IEmployeeScheduleRepository,
            EmployeeScheduleRepository>();

        services.AddScoped<
            IAttendanceLogRepository,
            AttendanceLogRepository>();

        services.AddScoped<
            IBiometricProfileRepository,
            BiometricProfileRepository>();

        services.AddScoped<
            IRoleRepository,
            RoleRepository>();

        services.AddScoped<
            IScheduleRequestRepository,
            ScheduleRequestRepository>();

        services.AddScoped<
            IProfileUpdateRequestRepository,
            ProfileUpdateRequestRepository>();

        services.AddScoped<
            ILeaveRequestRepository,
            LeaveRequestRepository>();

        // SignalR Services
        services.AddScoped<
            INotificationService,
            NotificationService>();

        // Device Integration
        var deviceMode = configuration.GetValue<string>("DeviceSettings:Mode");
        if (deviceMode == "Real")
        {
            services.AddScoped<IDeviceClientService, HikvisionISAPIClient>();
        }
        else if (deviceMode == "IsapiMock")
        {
            services.AddScoped<IDeviceClientService>(sp =>
            {
                var logger = sp.GetRequiredService<ILogger<HikvisionISAPIClient>>();
                var handler = new MockIsapiHandler();
                return new HikvisionISAPIClient(logger, handler);
            });
        }
        else
        {
            services.AddScoped<IDeviceClientService, MockDeviceClientService>();
        }

        // Jobs
        services.AddScoped<
            AttendanceReminderJob>();

        services.AddScoped<
            NotificationCleanupJob>();

        services.AddScoped<
            AttendanceSyncJob>();

        services.AddScoped<
            DeviceHealthCheckJob>();

        // Password hasher (used by Auth & User handlers)
        services.AddScoped<
            IPasswordHasher<User>,
            PasswordHasher<User>>();

        // Services
        services.AddScoped<
            IJwtService,
            JwtService>();

        return services;
    }
}
