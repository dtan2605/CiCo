using MediatR;
using cico.Application.DTOs.BiometricProfiles;
using cico.Domain.Enums;

namespace cico.Application.Features.BiometricProfiles
    .Queries.GetBiometricProfilesPaging;

public record GetBiometricProfilesPagingQuery(
    int PageNumber = 1,
    int PageSize = 20,
    Guid? EmployeeId = null,
    BiometricType? Type = null,
    bool? IsActive = null
) : IRequest<List<BiometricProfileDto>>;
