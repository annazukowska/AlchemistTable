using AlchemistTable.Core.Entities;

namespace AlchemistTable.Core.Interfaces
{
    public interface ICrudService<T>
        where T : EntityBase
    {
        Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
        Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<bool> AddAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> UpdateAsync(T entity, CancellationToken cancellationToken = default);
        Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
    }
}
