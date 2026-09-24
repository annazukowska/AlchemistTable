using AlchemistTable.Core.Entities;

namespace AlchemistTable.Core.Interfaces
{
    public interface IAlchemistService : ICrudService<Alchemist>
    {
        Task<Alchemist?> GetAlchemistByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<bool> UpdateAlchemistInventoryAsync(Guid id, List<Ingredient> inventory, CancellationToken cancellationToken = default);
        Task<bool> UpdateAlchemistPasswordAsync(Guid id, string newPassword, CancellationToken cancellationToken = default);
        Task<Alchemist?> LoginAlchemist(string email, string password, CancellationToken cancellationToken = default);
    }
}
