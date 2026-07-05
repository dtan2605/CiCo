using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;


namespace cico.Infrastructure.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly CICODbContext _context;

    public RefreshTokenRepository(CICODbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task<RefreshToken?>
        GetByTokenAsync(
            string token,
            bool includeUser = false)
    {
        IQueryable<RefreshToken> query =
            _context.RefreshTokens;

        if (includeUser)
        {
            query = query
                .Include(x => x.User)
                    .ThenInclude(x => x.Role)
                        .ThenInclude(x => x.RolePermissions)
                            .ThenInclude(x => x.Permission);
        }

        return await query
            .FirstOrDefaultAsync(
                x => x.Token == token);
    }
    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task<List<RefreshToken>> GetByUserIdAsync(Guid userId)
    {
        return await _context.RefreshTokens
            .Where(x => x.UserId == userId && !x.IsRevoked).ToListAsync();
    }

    public async Task UpdateRangeAsync(
        IEnumerable<RefreshToken> tokens
    ){
        _context.RefreshTokens.UpdateRange(tokens);
        await _context.SaveChangesAsync();
    }
}