using Microsoft.EntityFrameworkCore;
using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Entities;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Repositories;

public class UserRepository
    : IUserRepository
{
    private readonly CICODbContext _context;

    public UserRepository(
        CICODbContext context)
    {
        _context = context;
    }

    public async Task<User?> GetByIdAsync(
        Guid id)
    {
        return await _context.Users
            .Include(x => x.Role)
            .Include(x => x.Employee)
            .FirstOrDefaultAsync(x =>
                x.Id == id &&
                !x.IsDeleted);
    }

    public async Task<User?>
        GetByEmailAsync(string email)
    {
        return await _context.Users
            .Include(x => x.Role)
            .FirstOrDefaultAsync(x =>
                x.Email == email &&
                !x.IsDeleted);
    }

    public async Task<User?>
        GetByUsernameAsync(
            string username,
            bool includeRole = false)
    {
        IQueryable<User> query =
            _context.Users;

        if (includeRole)
        {
            query = query
                .Include(x => x.Role)
                    .ThenInclude(x => x.RolePermissions)
                        .ThenInclude(x => x.Permission);
        }

        return await query
            .FirstOrDefaultAsync(x =>
                x.Username == username &&
                !x.IsDeleted);
    }

    public async Task<User?>
        GetByEmailVerificationTokenAsync(
            string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x =>
                x.EmailVerificationToken ==
                token);
    }

    public async Task<User?>
        GetByPasswordResetTokenAsync(
            string token)
    {
        return await _context.Users
            .FirstOrDefaultAsync(x =>
                x.PasswordResetToken ==
                token);
    }

    public async Task<List<User>>
        GetPagingAsync(
            int pageNumber,
            int pageSize,
            string? keyword)
    {
        var query =
            _context.Users
                .Include(x => x.Role)
                .Where(x =>
                    !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(
            keyword))
        {
            query =
                query.Where(x =>
                    x.Username.Contains(
                        keyword)
                    || x.Email.Contains(
                        keyword));
        }

        return await query
            .OrderBy(x => x.Username)
            .Skip((pageNumber - 1)
                * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddAsync(
        User user)
    {
        await _context.Users.AddAsync(
            user);

        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(
        User user)
    {
        _context.Users.Update(user);

        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(
        User user)
    {
        _context.Users.Remove(user);

        await _context.SaveChangesAsync();
    }

    public async Task<bool>
        ExistsEmailAsync(
            string email)
    {
        return await _context.Users
            .AnyAsync(x =>
                x.Email == email &&
                !x.IsDeleted);
    }

    public async Task<bool>
        ExistsUsernameAsync(
            string username)
    {
        return await _context.Users
            .AnyAsync(x =>
                x.Username ==
                username &&
                !x.IsDeleted);
    }

    public async Task<int>
        CountAsync(
            string? keyword)
    {
        var query =
            _context.Users
                .Where(x =>
                    !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(
            keyword))
        {
            query =
                query.Where(x =>
                    x.Username.Contains(
                        keyword)
                    || x.Email.Contains(
                        keyword));
        }

        return await query.CountAsync();
    }

    public async Task<User?>
        GetByEmailWithPermissionsAsync(
            string email)
    {
        return await _context.Users
            .Include(x => x.Role)
                .ThenInclude(x =>
                    x.RolePermissions)
                        .ThenInclude(x =>
                            x.Permission)
            .FirstOrDefaultAsync(x =>
                x.Email == email &&
                !x.IsDeleted);
    }
}