using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Domain.Exceptions;
using cico.Application.DTOs.BiometricProfiles;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.BiometricProfiles
    .Commands.RegisterBiometric;

public class RegisterBiometricHandler
    : IRequestHandler<
        RegisterBiometricCommand,
        BiometricProfileDto>
{
    private readonly IBiometricProfileRepository _repository;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IMapper _mapper;

    public RegisterBiometricHandler(
        IBiometricProfileRepository repository,
        IEmployeeRepository employeeRepository,
        IMapper mapper)
    {
        _repository = repository;
        _employeeRepository = employeeRepository;
        _mapper = mapper;
    }

    public async Task<BiometricProfileDto> Handle(
        RegisterBiometricCommand request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _employeeRepository
                .GetByIdAsync(request.EmployeeId);

        if (employee == null)
            throw new DomainException(
                "Employee not found");

        var existing =
            await _repository
                .GetByEmployeeAndTypeAsync(
                    request.EmployeeId,
                    request.Type);

        if (existing != null)
        {
            existing.Template = request.Template;
            existing.RegisteredAt = DateTime.UtcNow;
            existing.IsActive = true;
            await _repository.UpdateAsync(existing);
            return _mapper.Map<BiometricProfileDto>(
                existing);
        }

        var profile = new BiometricProfile
        {
            EmployeeId = request.EmployeeId,
            Type = request.Type,
            Template = request.Template,
            IsActive = true,
            RegisteredAt = DateTime.UtcNow
        };

        await _repository.AddAsync(profile);

        return _mapper.Map<BiometricProfileDto>(
            profile);
    }
}
