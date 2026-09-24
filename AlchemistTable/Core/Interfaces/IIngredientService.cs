using AlchemistTable.Core.Entities;

namespace AlchemistTable.Core.Interfaces
{
    public interface IIngredientService : ICrudService<Ingredient>
    {
        Task<IReadOnlyList<Ingredient>> GetIngredientsByNameAsync(string name, CancellationToken cancellationToken = default);

    }
}
