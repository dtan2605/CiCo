using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using cico.Application.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace cico.Infrastructure.Auth;

public class JwtService : IJwtService
{
    private readonly JwtSettings _settings;

    public JwtService(IOptions<JwtSettings> settings)
    {
        _settings = settings.Value;
    }

    public string GenerateToken(
        Guid userId,
        string username,
        string role,
        IEnumerable<string>
            permissions)
    {
        var claims =
            new List<Claim>
            {
                new(
                    ClaimTypes.NameIdentifier,
                    userId.ToString()),

                new(
                    ClaimTypes.Name,
                    username),

                new(
                    ClaimTypes.Role,
                    role)
            };

        claims.AddRange(
            permissions.Select(p =>
                new Claim(
                    "permission",
                    p)));

        var key =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _settings.SecretKey));

        var creds =
            new SigningCredentials(
                key,
                SecurityAlgorithms
                    .HmacSha256);

        var token =
            new JwtSecurityToken(
                issuer:
                    _settings.Issuer,
                audience:
                    _settings.Audience,
                claims:
                    claims,
                expires:
                    DateTime.UtcNow
                        .AddMinutes(
                            _settings
                                .ExpiryMinutes),
                signingCredentials:
                    creds);

        return
            new JwtSecurityTokenHandler()
                .WriteToken(token);
    }

        public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = System.Security.Cryptography.RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);

        return Convert.ToBase64String(randomBytes);
    }

    public ClaimsPrincipal? GetPrincipalFromToken(string token)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_settings.SecretKey);

        try
        {
            var principal = tokenHandler.ValidateToken(token,
                new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = _settings.Issuer,
                    ValidAudience = _settings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(key)
                },
                out _);

            return principal;
        }
        catch
        {
            return null;
        }
    }
}