using cico.Domain.Entities;
using MediatR;
using cico.Application.DTOs.Users;
namespace cico.Application.Features.Users.Queries.GetUserById;
public record GetUserByIdQuery(Guid Id)
    : IRequest<UserDto>;