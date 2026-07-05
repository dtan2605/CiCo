using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Application.DTOs.EmployeeSchedules;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.EmployeeSchedules
    .Commands.CreateEmployeeSchedule;

public class CreateEmployeeScheduleHandler
    : IRequestHandler<
        CreateEmployeeScheduleCommand,
        EmployeeScheduleDto>
{
    private readonly IEmployeeScheduleRepository _repository;
    private readonly IMapper _mapper;

    public CreateEmployeeScheduleHandler(
        IEmployeeScheduleRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<EmployeeScheduleDto> Handle(
        CreateEmployeeScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var entity = new EmployeeSchedule
        {
            EmployeeId = request.EmployeeId,
            ScheduleId = request.ScheduleId,
            WorkDate = request.WorkDate,
            IsOvertime = request.IsOvertime,
            Note = request.Note ?? string.Empty
        };

        await _repository.AddAsync(entity);
        await _repository.SaveChangesAsync();

        return _mapper.Map<EmployeeScheduleDto>(entity);
    }
}
