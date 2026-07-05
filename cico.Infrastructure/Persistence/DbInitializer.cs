using cico.Domain.Constants;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence.SeedData;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace cico.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(CICODbContext context)
    {
        if ((await context.Database.GetPendingMigrationsAsync()).Any())
            await context.Database.MigrateAsync();

        if (!await context.Roles.AnyAsync())
        {
            context.Roles.AddRange(RoleSeed.Get());
            await context.SaveChangesAsync();
        }

        if (!await context.Departments.AnyAsync())
        {
            context.Departments.AddRange(DepartmentSeed.Get());
            await context.SaveChangesAsync();
        }

        if (!await context.Positions.AnyAsync())
        {
            context.Positions.AddRange(PositionSeed.Get());
            await context.SaveChangesAsync();
        }

        if (!await context.Schedules.AnyAsync())
        {
            context.Schedules.AddRange(ScheduleSeed.Get());
            await context.SaveChangesAsync();
        }

        if (!await context.Users.AnyAsync())
        {
            var hasher = new PasswordHasher<User>();

            var admin = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000001"),
                Username = "admin",
                Email = "admin@cico.com",
                RoleId = DefaultRoleIds.Admin,
                IsActive = true,
                IsDeleted = false,
                IsLocked = false,
                FailedLoginCount = 0,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            admin.PasswordHash = hasher.HashPassword(admin, "Admin@123");

            context.Users.Add(admin);

            var manager = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000002"),
                Username = "manager",
                Email = "manager@cico.com",
                RoleId = DefaultRoleIds.Manager,
                IsActive = true,
                IsDeleted = false,
                IsLocked = false,
                FailedLoginCount = 0,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            manager.PasswordHash = hasher.HashPassword(manager, "Manager@123");

            context.Users.Add(manager);

            var employeeUser = new User
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000003"),
                Username = "employee",
                Email = "employee@cico.com",
                RoleId = DefaultRoleIds.Employee,
                IsActive = true,
                IsDeleted = false,
                IsLocked = false,
                FailedLoginCount = 0,
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };
            employeeUser.PasswordHash = hasher.HashPassword(employeeUser, "Employee@123");

            context.Users.Add(employeeUser);

            await context.SaveChangesAsync();

            var deptIt = Guid.Parse("BBBBBBBB-BBBB-BBBB-BBBB-BBBBBBBBBBBB");
            var posStaff = Guid.Parse("DDDDDDDD-DDDD-DDDD-DDDD-DDDDDDDDDDDD");
            var posManager = Guid.Parse("FFFFFFFF-FFFF-FFFF-FFFF-FFFFFFFFFFFF");

            context.Employees.AddRange(
                new Employee
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000011"),
                    EmployeeCode = "ADM-001",
                    FullName = "System Admin",
                    Email = "admin@cico.com",
                    HireDate = DateTime.UtcNow,
                    UserId = admin.Id,
                    DepartmentId = deptIt,
                    PositionId = posManager,
                    IsActive = true
                },
                new Employee
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000012"),
                    EmployeeCode = "MGR-001",
                    FullName = "Manager User",
                    Email = "manager@cico.com",
                    HireDate = DateTime.UtcNow,
                    UserId = manager.Id,
                    DepartmentId = deptIt,
                    PositionId = posStaff,
                    IsActive = true
                },
                new Employee
                {
                    Id = Guid.Parse("00000000-0000-0000-0000-000000000013"),
                    EmployeeCode = "EMP-001",
                    FullName = "Employee User",
                    Email = "employee@cico.com",
                    HireDate = DateTime.UtcNow,
                    UserId = employeeUser.Id,
                    DepartmentId = deptIt,
                    PositionId = posStaff,
                    IsActive = true
                }
            );

            await context.SaveChangesAsync();
        }

        if (!await context.Devices.AnyAsync())
        {
            context.Devices.Add(new Device
            {
                Id = Guid.Parse("00000000-0000-0000-0000-000000000021"),
                DeviceCode = "HIK-MOCK-001",
                Name = "Main Gate Mock Device",
                Location = "Main Entrance",
                IpAddress = "192.168.1.100",
                Port = 80,
                SerialNumber = "MOCK-SN-001",
                Manufacturer = "Hikvision",
                FirmwareVersion = "V1.0.0",
                Username = "admin",
                Password = "admin",
                Status = Domain.Enums.DeviceStatus.Online,
                IsActive = true
            });

            await context.SaveChangesAsync();
        }
    }
}
