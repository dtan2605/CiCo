using Microsoft.Extensions.Configuration;

namespace cico.Infrastructure.Auth;

public class JwtSettings
{
    public const string SectionName = "JwtSettings";

    [ConfigurationKeyName("Secret")]
    public string SecretKey { get; set; } = string.Empty;

    public string Issuer { get; set; } = string.Empty;

    public string Audience { get; set; } = string.Empty;

    [ConfigurationKeyName("ExpiryInMinutes")]
    public int ExpiryMinutes { get; set; }
}
