using MediatR;
using cico.Application.DTOs.Employees;

namespace cico.Application.Features.Employees.Queries.GetEmployeesPaging;

public record GetEmployeesPagingQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Keyword = null
) : IRequest<List<EmployeeDto>>;