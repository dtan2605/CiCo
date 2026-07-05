using AutoMapper;
using MediatR;
using cico.Application.DTOs.Employees;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Employees.Queries.GetEmployeesPaging;

public class GetEmployeesPagingHandler
    : IRequestHandler<
        GetEmployeesPagingQuery,
        List<EmployeeDto>>
{
    private readonly IEmployeeRepository _repository;
    private readonly IMapper _mapper;

    public GetEmployeesPagingHandler(
        IEmployeeRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<EmployeeDto>>
        Handle(
            GetEmployeesPagingQuery request,
            CancellationToken cancellationToken)
    {
        var employees =
            await _repository.GetPagingAsync(
                request.PageNumber,
                request.PageSize,
                request.Keyword);

        return _mapper.Map<List<EmployeeDto>>(
            employees);
    }
}