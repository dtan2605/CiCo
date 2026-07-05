namespace cico.Application.DTOs.Users;

public class UserDto
{
    public Guid Id { get; set; }

    public string Username { get; set; }
        = string.Empty;

    public string Email { get; set; }
        = string.Empty;

    public bool IsActive { get; set; }

    public Guid RoleId { get; set; }

    public string RoleName { get; set; }
        = string.Empty;

    public DateTime CreatedAt { get; set; }
}