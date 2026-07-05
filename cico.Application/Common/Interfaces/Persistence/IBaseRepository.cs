using cico.Domain.Entities;
using cico.Domain.Common;
namespace cico.Application.Common.Interfaces.Persistence;

public interface IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    Task<TEntity?> GetByIdAsync(Guid id);

    Task<List<TEntity>> GetAllAsync();

    Task AddAsync(TEntity entity);

    Task UpdateAsync(TEntity entity);

    Task DeleteAsync(TEntity entity);
    void Update(TEntity entity);

    void Delete(TEntity entity);

    Task<bool> ExistsAsync(Guid id);

    Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default);
}