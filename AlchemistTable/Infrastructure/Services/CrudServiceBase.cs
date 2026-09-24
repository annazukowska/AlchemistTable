using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlchemistTable.Infrastructure.Services
{
    public class CrudServiceBase<T, TContext> : ICrudService<T>
        where T : EntityBase
        where TContext : DbContext
    {
        private readonly DbContext _appDbContext;

        public CrudServiceBase(DbContext appDbContext)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }
            await _appDbContext.Set<T>().AddAsync(entity, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var entity = await _appDbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
            if (entity == null)
            {
                return false;
            }
            _appDbContext.Set<T>().Remove(entity);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Set<T>().ToListAsync(cancellationToken);
        }

        public async Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            if (id == Guid.Empty)
            {
                throw new ArgumentException("ID cannot be empty.", nameof(id));
            }
            return await _appDbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity));

            var id = entity.Id;
            var existing = await _appDbContext.Set<T>().FindAsync(new object[] { id }, cancellationToken);
            if (existing == null)
                return false;

            _appDbContext.Entry(existing).CurrentValues.SetValues(entity);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
    }
}
