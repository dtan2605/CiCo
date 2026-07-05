using AutoMapper;
using MediatR;
using cico.Application.DTOs.Departments;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Departments.Queries.GetDepartmentsPaging;

public class GetDepartmentsPagingHandler
    : IRequestHandler<
        GetDepartmentsPagingQuery,
        List<DepartmentDto>>
{
    private readonly IDepartmentRepository _repository;
    private readonly IMapper _mapper;

    public GetDepartmentsPagingHandler(
        IDepartmentRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<DepartmentDto>>
        Handle(
            GetDepartmentsPagingQuery request,
            CancellationToken cancellationToken)
    {
        var departments =
            await _repository.GetPagingAsync(
                request.PageNumber,
                request.PageSize,
                request.Keyword);

        return _mapper.Map<List<DepartmentDto>>(
            departments);
    }
}
