using cico.Application.Interfaces;
using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Enums;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Jobs;

public class DeviceHealthCheckJob
{
    private readonly CICODbContext _db;
    private readonly IDeviceRepository _deviceRepo;
    private readonly IDeviceClientService _deviceClient;

    public DeviceHealthCheckJob(
        CICODbContext db,
        IDeviceRepository deviceRepo,
        IDeviceClientService deviceClient)
    {
        _db = db;
        _deviceRepo = deviceRepo;
        _deviceClient = deviceClient;
    }

    public async Task RunAsync()
    {
        var devices = await _deviceRepo.GetActiveDevicesAsync();

        foreach (var device in devices)
        {
            try
            {
                var result = await _deviceClient.TestConnectionAsync(
                    device.IpAddress, device.Port,
                    device.Username ?? "admin",
                    device.Password ?? "admin");

                device.Status = result.Status;
                if (result.Success)
                    device.LastSyncTime = DateTime.UtcNow;
            }
            catch
            {
                device.Status = DeviceStatus.Offline;
            }
        }

        await _db.SaveChangesAsync();
    }
}
