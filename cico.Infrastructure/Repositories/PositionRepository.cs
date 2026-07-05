using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Repositories;

public class PositionRepository
    : BaseRepository<Position>,
      IPositionRepository
{
    public PositionRepository(
        CICODbContext context)
        : base(context)
    {
    }
}