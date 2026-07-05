using AutoMapper;
using MediatR;
using cico.Application.DTOs.EmployeeSchedules;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.EmployeeSchedules
    .Queries.GetEmployeeScheduleById;

public class GetEmployeeScheduleByIdHandler
    : IRequestHandler<
        GetEmployeeScheduleByIdQuery,
        EmployeeScheduleDto?>
{
    private readonly IEmployeeScheduleRepository _repository;
    private readonly IMapper _mapper;

    public GetEmployeeScheduleByIdHandler(
        IEmployeeScheduleRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EmployeeScheduleDto?> Handle(
        GetEmployeeScheduleByIdQuery request,
        CancellationToken cancellationToken)
    {
        var entity =
            await _repository.GetByIdAsync(
                request.Id);

        if (entity == null)
            return null;

        return _mapper.Map<EmployeeScheduleDto>(
            entity);
    }
}
