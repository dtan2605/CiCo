using cico.Domain.Entities;

namespace cico.Application.Common.Interfaces.Persistence;

public interface IUserRepository
{
    Task<User?> GetByIdAsync(Guid id);

    Task<User?> GetByEmailAsync(string email);
    
    Task<User?> GetByEmailWithPermissionsAsync(string email);

    Task<User?> GetByUsernameAsync(string username, bool includeRole = false);

    Task<User?> GetByEmailVerificationTokenAsync(
        string token);

    Task<User?> GetByPasswordResetTokenAsync(
        string token);

    Task<List<User>> GetPagingAsync(
        int pageNumber,
        int pageSize,
        string? keyword);

    Task AddAsync(User user);

    Task UpdateAsync(User user);

    Task DeleteAsync(User user);

    Task<bool> ExistsEmailAsync(string email);

    Task<bool> ExistsUsernameAsync(
        string username);

    Task<int> CountAsync(
        string? keyword);
}