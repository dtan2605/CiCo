namespace cico.Application.DTOs.Users;

public class UpdateUserDto
{
    public Guid Id { get; set; }

    public string Email { get; set; }
        = string.Empty;

    public bool IsActive { get; set; }

    public Guid RoleId { get; set; }
}