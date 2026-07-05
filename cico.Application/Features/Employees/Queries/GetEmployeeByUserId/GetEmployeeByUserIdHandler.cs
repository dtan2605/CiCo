using AutoMapper;
using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.DTOs.Employees;

namespace cico.Application.Features.Employees.Queries.GetEmployeeByUserId;

public class GetEmployeeByUserIdHandler : IRequestHandler<GetEmployeeByUserIdQuery, EmployeeDto?>
{
    private readonly IEmployeeRepository _repo;
    private readonly IMapper _mapper;

    public GetEmployeeByUserIdHandler(IEmployeeRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<EmployeeDto?> Handle(GetEmployeeByUserIdQuery request, CancellationToken cancellationToken)
    {
        var employee = await _repo.GetByUserIdAsync(request.UserId);
        if (employee == null) return null;

        var detail = await _repo.GetDetailAsync(employee.Id);
        return _mapper.Map<EmployeeDto>(detail);
    }
}
