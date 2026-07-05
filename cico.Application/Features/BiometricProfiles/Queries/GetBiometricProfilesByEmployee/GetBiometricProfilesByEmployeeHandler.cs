using AutoMapper;
using MediatR;
using cico.Application.DTOs.BiometricProfiles;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfilesByEmployee;

public class GetBiometricProfilesByEmployeeHandler
    : IRequestHandler<
        GetBiometricProfilesByEmployeeQuery,
        List<BiometricProfileDto>>
{
    private readonly IBiometricProfileRepository _repository;
    private readonly IMapper _mapper;

    public GetBiometricProfilesByEmployeeHandler(
        IBiometricProfileRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<List<BiometricProfileDto>> Handle(
        GetBiometricProfilesByEmployeeQuery request,
        CancellationToken cancellationToken)
    {
        var profiles =
            await _repository
                .GetByEmployeeIdAsync(
                    request.EmployeeId);

        return _mapper.Map<List<BiometricProfileDto>>(
            profiles);
    }
}
