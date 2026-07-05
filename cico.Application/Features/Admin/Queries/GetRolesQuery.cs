using MediatR;
using cico.Application.DTOs.Admin;

namespace cico.Application.Features.Admin.Queries;

public record GetRolesQuery : IRequest<List<RoleDto>>;
