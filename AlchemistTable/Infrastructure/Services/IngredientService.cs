using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AlchemistTable.Infrastructure.Services
{
    public class IngredientService : IIngredientService
    {
        private readonly AppDbContext _appDbContext;
        public IngredientService(AppDbContext appDbContext) 
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
        }

        public async Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Ingredients.ToListAsync(cancellationToken);
        }

        public async Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Ingredients.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IReadOnlyList<Ingredient>> GetIngredientsByNameAsync(string name, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Ingredients
                .Where(i => EF.Functions.Like(i.Name, $"%{name}%"))
                .ToListAsync(cancellationToken);
        }
        public async Task<bool> AddAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
        {
            await _appDbContext.Ingredients.AddAsync(ingredient, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var ingredient = await _appDbContext.Ingredients.FindAsync(new object[] { id }, cancellationToken);
            if(ingredient != null)
            {
                _appDbContext.Ingredients.Remove(ingredient);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;
        }

        public async Task<bool> UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken = default)
        {
            var existing = await _appDbContext.Ingredients.FindAsync(new object[] { ingredient.Id }, cancellationToken);
            if (existing != null)
            {
                existing.Name = ingredient.Name;
                existing.Rarity = ingredient.Rarity;
                existing.Effect = ingredient.Effect;
                existing.Description = ingredient.Description;

                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                throw new KeyNotFoundException($"Ingredient with ID {ingredient.Id} not found.");
            }
        }
    }
}
