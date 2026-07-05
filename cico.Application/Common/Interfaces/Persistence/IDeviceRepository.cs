using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;
public interface IDeviceRepository
    : IBaseRepository<Device>
{
    Task<Device?> GetByCodeAsync(
        string deviceCode);

    Task<List<Device>> GetActiveDevicesAsync();
}