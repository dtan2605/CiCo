using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IRefreshTokenRepository
{
    Task AddAsync(RefreshToken token);
    Task<RefreshToken?> GetByTokenAsync(string token, bool includeUser = false);
    Task UpdateAsync(RefreshToken token);
    Task<List<RefreshToken>> GetByUserIdAsync(Guid userId);
    Task UpdateRangeAsync(IEnumerable<RefreshToken> tokens);
}