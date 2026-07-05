using AutoMapper;
using MediatR;
using cico.Domain.Entities;
using cico.Application.DTOs.Schedules;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Schedules.Commands.CreateSchedule;

public class CreateScheduleHandler
    : IRequestHandler<CreateScheduleCommand, ScheduleDto>
{
    private readonly IScheduleRepository _repository;
    private readonly IMapper _mapper;

    public CreateScheduleHandler(
        IScheduleRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ScheduleDto> Handle(
        CreateScheduleCommand request,
        CancellationToken cancellationToken)
    {
        var schedule = new Schedule
        {
            Name = request.Name,
            StartTime = request.StartTime,
            EndTime = request.EndTime,
            LateThresholdMinutes = request.LateThresholdMinutes,
            IsActive = true
        };

        await _repository.AddAsync(schedule);
        await _repository.SaveChangesAsync();

        return _mapper.Map<ScheduleDto>(schedule);
    }
}
