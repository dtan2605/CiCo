using cico.Application.Common.Interfaces.Persistence;
using cico.Domain.Common;
using Microsoft.EntityFrameworkCore;
using cico.Infrastructure.Persistence;

namespace cico.Infrastructure.Repositories;
public class BaseRepository<TEntity>
    : IBaseRepository<TEntity>
    where TEntity : BaseEntity
{
    protected readonly CICODbContext _context;
    protected readonly DbSet<TEntity> _dbSet;

    public BaseRepository(
        CICODbContext context)
    {
        _context = context;
        _dbSet = context.Set<TEntity>();
    }

    public virtual async Task<TEntity?> GetByIdAsync(
        Guid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public virtual async Task<List<TEntity>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public virtual async Task AddAsync(
        TEntity entity)
    {
        await _dbSet.AddAsync(entity);
        await _context.SaveChangesAsync();
    }

    public virtual void Update(
        TEntity entity)
    {
        _dbSet.Update(entity);
    }

    public virtual void Delete(
        TEntity entity)
    {
        _dbSet.Remove(entity);
    }

    public virtual async Task<bool> ExistsAsync(
        Guid id)
    {
        return await _dbSet
            .AnyAsync(x => x.Id == id);
    }

    public virtual Task<int> SaveChangesAsync(
        CancellationToken cancellationToken = default)
    {
        return _context.SaveChangesAsync(
            cancellationToken);
    }

    public virtual async Task UpdateAsync(
        TEntity entity)
    {
        _dbSet.Update(entity);
        await _context.SaveChangesAsync();
    }

    public virtual async Task DeleteAsync(
        TEntity entity)
    {
        _dbSet.Remove(entity);
        await _context.SaveChangesAsync();
    }
}