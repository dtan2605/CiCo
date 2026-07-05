using MediatR;
using cico.Application.DTOs.Users;

namespace cico.Application.Features.Users.Queries.GetUsersPaging
{
    public record GetUsersPagingQuery(
    int PageNumber,
    int PageSize,
    string? Keyword
) : IRequest<List<UserDto>>;
}