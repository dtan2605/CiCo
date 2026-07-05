using AutoMapper;
using MediatR;
using cico.Application.DTOs.Employees;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Employees.Queries.GetEmployeeById;

public class GetEmployeeByIdHandler
    : IRequestHandler<GetEmployeeByIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;

    public GetEmployeeByIdHandler(
        IEmployeeRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EmployeeDto?> Handle(
        GetEmployeeByIdQuery request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _repository.GetDetailAsync(
                request.Id);

        if (employee == null)
            return null;

        return _mapper.Map<EmployeeDto>(
            employee);
    }
}