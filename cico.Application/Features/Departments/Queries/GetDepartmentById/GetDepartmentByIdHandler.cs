using AutoMapper;
using MediatR;
using cico.Application.DTOs.Departments;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Departments.Queries.GetDepartmentById;

public class GetDepartmentByIdHandler
    : IRequestHandler<GetDepartmentByIdQuery, DepartmentDto?>
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;

    public GetDepartmentByIdHandler(
        IDepartmentRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<DepartmentDto?> Handle(
        GetDepartmentByIdQuery request,
        CancellationToken cancellationToken)
    {
        var department =
            await _repository.GetDetailAsync(
                request.Id);

        if (department == null)
            return null;

        return _mapper.Map<DepartmentDto>(
            department);
    }
}
