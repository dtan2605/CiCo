using MediatR;
using cico.Domain.Constants;
using cico.Domain.Entities;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;
using Microsoft.AspNetCore.Identity;

namespace cico.Application.Features.Admin.Commands;

public class CreateAdminUserHandler : IRequestHandler<CreateAdminUserCommand, Guid>
{
    private readonly IUserRepository _userRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IPasswordHasher<User> _hasher;

    public CreateAdminUserHandler(
        IUserRepository userRepo,
        IEmployeeRepository employeeRepo,
        IPasswordHasher<User> hasher)
    {
        _userRepo = userRepo;
        _employeeRepo = employeeRepo;
        _hasher = hasher;
    }

    public async Task<Guid> Handle(CreateAdminUserCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepo.ExistsEmailAsync(request.Email))
            throw new DomainException("Email already exists");

        if (await _userRepo.ExistsUsernameAsync(request.Username))
            throw new DomainException("Username already exists");

        if (!string.IsNullOrWhiteSpace(request.EmployeeCode))
        {
            var existing = await _employeeRepo.GetByEmployeeCodeAsync(request.EmployeeCode);
            if (existing != null)
                throw new DomainException("Employee code already exists");
        }

        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            RoleId = request.RoleId,
            IsActive = true,
            IsDeleted = false,
            IsLocked = false,
            FailedLoginCount = 0,
            EmailConfirmed = true,
            SecurityStamp = Guid.NewGuid().ToString()
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        await _userRepo.AddAsync(user);

        var employee = new Employee
        {
            EmployeeCode = request.EmployeeCode ?? $"EMP-{user.Id:N}"[..12],
            FullName = request.FullName,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber ?? string.Empty,
            HireDate = DateTime.UtcNow,
            UserId = user.Id,
            DepartmentId = request.DepartmentId ?? DefaultDepartmentIds.Default,
            PositionId = request.PositionId ?? DefaultPositionIds.Staff,
            IsActive = true,
            IsDeleted = false
        };

        await _employeeRepo.AddAsync(employee);
        await _employeeRepo.SaveChangesAsync();

        return user.Id;
    }
}
