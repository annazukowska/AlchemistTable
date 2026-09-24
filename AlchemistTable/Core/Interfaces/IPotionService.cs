using AlchemistTable.Application.DTOs;
using AlchemistTable.Core.Entities;

namespace AlchemistTable.Core.Interfaces
{
    public interface IPotionService : ICrudService<Potion>
    {
        Task<Potion?> BrewPotion(List<Ingredient> ingredients, CraftPotionRequest request, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Potion>> GetPotionsByAlchemistAsync(string alchemistId, CancellationToken cancellationToken = default);
    }
}
