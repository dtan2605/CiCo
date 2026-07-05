using AutoMapper;
using MediatR;
using cico.Application.Common.Interfaces.Persistence;
using cico.Application.DTOs.Users;

namespace cico.Application.Features.Users.Queries.GetUserById;

public class GetUserByIdHandler
    : IRequestHandler<
        GetUserByIdQuery,
        UserDto?>
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;

    public GetUserByIdHandler(
        IUserRepository repo,
        IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<UserDto?> Handle(
        GetUserByIdQuery request,
        CancellationToken cancellationToken)
    {
        var user =
            await _repo.GetByIdAsync(
                request.Id);

        if (user == null)
            return null;

        return _mapper.Map<UserDto>(
            user);
    }
}