using AutoMapper;
using MediatR;
using cico.Application.DTOs.Users;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Users.Queries.GetUsersPaging;

public class GetUsersPagingHandler
    : IRequestHandler<
        GetUsersPagingQuery,
        List<UserDto>>
{
    private readonly IUserRepository _repo;
    private readonly IMapper _mapper;

    public GetUsersPagingHandler(
        IUserRepository repo,
        IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<UserDto>>
        Handle(
            GetUsersPagingQuery request,
            CancellationToken cancellationToken)
    {
        var users =
            await _repo.GetPagingAsync(
                request.PageNumber,
                request.PageSize,
                request.Keyword);

        return _mapper.Map<
            List<UserDto>>(users);
    }
}