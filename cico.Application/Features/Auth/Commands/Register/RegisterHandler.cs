using MediatR;
using cico.Domain.Constants;
using cico.Domain.Entities;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;
using Microsoft.AspNetCore.Identity;

namespace cico.Application.Features.Auth.Commands.Register;

public class RegisterHandler : IRequestHandler<RegisterCommand, Guid>
{
    private readonly IUserRepository _userRepo;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IPasswordHasher<User> _hasher;

    public RegisterHandler(
        IUserRepository userRepo,
        IEmployeeRepository employeeRepo,
        IPasswordHasher<User> hasher)
    {
        _userRepo = userRepo;
        _employeeRepo = employeeRepo;
        _hasher = hasher;
    }

    public async Task<Guid> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        if (await _userRepo.ExistsEmailAsync(request.Email))
            throw new DomainException("Email already exists");

        var user = new User
        {
            Email = request.Email,
            Username = request.Email,
            RoleId = DefaultRoleIds.Employee
        };

        user.PasswordHash = _hasher.HashPassword(user, request.Password);

        await _userRepo.AddAsync(user);

        var employee = new Employee
        {
            EmployeeCode = $"EMP-{user.Id:N}"[..12],
            FullName = request.FullName,
            Email = request.Email,
            HireDate = DateTime.UtcNow,
            UserId = user.Id,
            DepartmentId = DefaultDepartmentIds.Default,
            PositionId = DefaultPositionIds.Staff
        };

        await _employeeRepo.AddAsync(employee);
        await _employeeRepo.SaveChangesAsync();

        return user.Id;
    }
}
