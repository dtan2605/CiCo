using AutoMapper;
using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.DTOs.Admin;

namespace cico.Application.Features.Admin.Queries;

public class GetRolesHandler : IRequestHandler<GetRolesQuery, List<RoleDto>>
{
    private readonly IRoleRepository _repo;
    private readonly IMapper _mapper;

    public GetRolesHandler(IRoleRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<RoleDto>> Handle(GetRolesQuery request, CancellationToken cancellationToken)
    {
        var roles = await _repo.GetAllAsync();
        return _mapper.Map<List<RoleDto>>(roles);
    }
}
