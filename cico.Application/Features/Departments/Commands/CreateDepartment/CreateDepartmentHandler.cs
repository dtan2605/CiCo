using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Application.DTOs.Departments;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Departments.Commands.CreateDepartment;

public class CreateDepartmentHandler
    : IRequestHandler<CreateDepartmentCommand, DepartmentDto>
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;

    public CreateDepartmentHandler(
        IDepartmentRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DepartmentDto> Handle(
        CreateDepartmentCommand request,
        CancellationToken cancellationToken)
    {
        var department = new Department
        {
            Name = request.Name,
            Description = request.Description
        };

        await _repository.AddAsync(department);

        return _mapper.Map<DepartmentDto>(department);
    }
}
