using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Application.DTOs.Employees;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Employees.Commands.CreateEmployee;

public class CreateEmployeeHandler
    : IRequestHandler<CreateEmployeeCommand, EmployeeDto>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;

    public CreateEmployeeHandler(
        IEmployeeRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EmployeeDto> Handle(
        CreateEmployeeCommand request,
        CancellationToken cancellationToken)
    {
        var employee = new Employee
        {
            EmployeeCode = request.EmployeeCode,
            FullName = request.FullName,
            DateOfBirth = request.DateOfBirth,
            Email = request.Email,
            PhoneNumber = request.PhoneNumber,
            Address = request.Address,
            DepartmentId = request.DepartmentId,
            PositionId = request.PositionId,
            UserId = request.UserId,
            IsActive = true,
            IsDeleted = false,
            HireDate = DateTime.UtcNow
        };

        await _repository.AddAsync(employee);

        return _mapper.Map<EmployeeDto>(employee);
    }
}