using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Repositories;

public class DeviceRepository
    : BaseRepository<Device>,
      IDeviceRepository
{
    public DeviceRepository(
        CICODbContext context)
        : base(context)
    {
    }

    public async Task<Device?>
        GetByCodeAsync(
            string deviceCode)
    {
        return await _context.Devices
            .AsNoTracking()
            .FirstOrDefaultAsync(
                x => x.DeviceCode == deviceCode);
    }

    public async Task<List<Device>> GetActiveDevicesAsync()
    {
        return await _context.Devices
            .Where(x => x.IsActive)
            .ToListAsync();
    }
}