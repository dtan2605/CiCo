using MediatR;
using cico.Domain.Entities;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Admin.Commands;

public class UpdateAdminUserHandler : IRequestHandler<UpdateAdminUserCommand>
{
    private readonly IUserRepository _userRepo;
    private readonly IEmployeeRepository _employeeRepo;

    public UpdateAdminUserHandler(
        IUserRepository userRepo,
        IEmployeeRepository employeeRepo)
    {
        _userRepo = userRepo;
        _employeeRepo = employeeRepo;
    }

    public async Task Handle(UpdateAdminUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepo.GetByIdAsync(request.Id);
        if (user == null)
            throw new DomainException("User not found");

        if (request.Username != null)
            user.Username = request.Username;

        if (request.Email != null)
            user.Email = request.Email;

        if (request.RoleId.HasValue)
            user.RoleId = request.RoleId.Value;

        if (request.IsActive.HasValue)
            user.IsActive = request.IsActive.Value;

        await _userRepo.UpdateAsync(user);

        var employee = await _employeeRepo.GetByUserIdAsync(request.Id);
        if (employee != null)
        {
            if (request.FullName != null)
                employee.FullName = request.FullName;

            if (request.PhoneNumber != null)
                employee.PhoneNumber = request.PhoneNumber;

            if (request.DepartmentId.HasValue)
                employee.DepartmentId = request.DepartmentId.Value;

            if (request.PositionId.HasValue)
                employee.PositionId = request.PositionId.Value;

            employee.Email = user.Email;

            _employeeRepo.Update(employee);
            await _employeeRepo.SaveChangesAsync();
        }
    }
}
