using AutoMapper;
using MediatR;
using cico.Application.DTOs.BiometricProfiles;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfilesPaging;

public class GetBiometricProfilesPagingHandler
    : IRequestHandler<
        GetBiometricProfilesPagingQuery,
        List<BiometricProfileDto>>
{
    private readonly IBiometricProfileRepository _repository;
    private readonly IMapper _mapper;

    public GetBiometricProfilesPagingHandler(
        IBiometricProfileRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<BiometricProfileDto>> Handle(
        GetBiometricProfilesPagingQuery request,
        CancellationToken cancellationToken)
    {
        var all =
            await _repository.GetAllAsync();

        var filtered = all
            .Where(x =>
                (!request.EmployeeId.HasValue ||
                    x.EmployeeId ==
                        request.EmployeeId.Value) &&
                (!request.Type.HasValue ||
                    x.Type == request.Type.Value) &&
                (!request.IsActive.HasValue ||
                    x.IsActive == request.IsActive.Value))
            .Skip((request.PageNumber - 1)
                * request.PageSize)
            .Take(request.PageSize)
            .ToList();

        return _mapper.Map<List<BiometricProfileDto>>(
            filtered);
    }
}
