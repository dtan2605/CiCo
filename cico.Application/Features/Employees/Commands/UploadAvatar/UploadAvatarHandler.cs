using MediatR;
using cico.Domain.Exceptions;
using cico.Application.Common.Interfaces.Persistence;

namespace cico.Application.Features.Employees.Commands.UploadAvatar;

public class UploadAvatarHandler
    : IRequestHandler<UploadAvatarCommand, string>
{
    private readonly IEmployeeRepository _repository;

    public UploadAvatarHandler(
        IEmployeeRepository repository)
    {
        _repository = repository;
    }

    public async Task<string> Handle(
        UploadAvatarCommand request,
        CancellationToken cancellationToken)
    {
        var employee =
            await _repository.GetByUserIdAsync(request.UserId);

        if (employee == null)
            throw new DomainException("Employee not found");

        var base64 = request.ImageBase64;
        if (string.IsNullOrWhiteSpace(base64))
            throw new DomainException("Image data is required");

        var commaIdx = base64.IndexOf(',');
        var data = commaIdx > 0
            ? base64[(commaIdx + 1)..]
            : base64;

        var imageBytes = Convert.FromBase64String(data);

        var dir = Path.Combine(
            Directory.GetCurrentDirectory(),
            "wwwroot", "uploads", "avatars");

        if (!Directory.Exists(dir))
            Directory.CreateDirectory(dir);

        var ext = ".png";
        if (commaIdx > 0)
        {
            var mime = base64[..commaIdx].ToLowerInvariant();
            if (mime.Contains("jpeg") || mime.Contains("jpg"))
                ext = ".jpg";
            else if (mime.Contains("gif"))
                ext = ".gif";
            else if (mime.Contains("webp"))
                ext = ".webp";
        }

        var fileName =
            $"{employee.Id}_{DateTimeOffset.UtcNow.ToUnixTimeMilliseconds()}{ext}";
        var filePath = Path.Combine(dir, fileName);

        await File.WriteAllBytesAsync(filePath, imageBytes, cancellationToken);

        var url = $"/uploads/avatars/{fileName}";
        employee.AvatarUrl = url;

        await _repository.UpdateAsync(employee);

        return url;
    }
}
