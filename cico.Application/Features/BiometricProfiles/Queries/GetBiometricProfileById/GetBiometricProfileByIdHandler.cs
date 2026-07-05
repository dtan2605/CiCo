using AutoMapper;
using MediatR;
using cico.Application.DTOs.BiometricProfiles;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfileById;

public class GetBiometricProfileByIdHandler
    : IRequestHandler<
        GetBiometricProfileByIdQuery,
        BiometricProfileDto?>
{
    private readonly IBiometricProfileRepository _repository;
    private readonly IMapper _mapper;

    public GetBiometricProfileByIdHandler(
        IBiometricProfileRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<BiometricProfileDto?> Handle(
        GetBiometricProfileByIdQuery request,
        CancellationToken cancellationToken)
    {
        var profile =
            await _repository.GetByIdAsync(
                request.Id);

        if (profile == null)
            return null;

        return _mapper.Map<BiometricProfileDto>(
            profile);
    }
}
