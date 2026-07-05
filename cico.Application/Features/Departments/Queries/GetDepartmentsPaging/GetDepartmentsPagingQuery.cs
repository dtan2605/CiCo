using MediatR;
using cico.Application.DTOs.Departments;

namespace cico.Application.Features.Departments.Queries.GetDepartmentsPaging;

public record GetDepartmentsPagingQuery(
    int PageNumber = 1,
    int PageSize = 10,
    string? Keyword = null
) : IRequest<List<DepartmentDto>>;
