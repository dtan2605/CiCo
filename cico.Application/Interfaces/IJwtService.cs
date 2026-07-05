using System.Security.Claims;

namespace cico.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(
        Guid userId, 
        string username, 
        string role, 
        IEnumerable<string> permissions);
    string GenerateRefreshToken();
}