using AlchemistTable.Application.DTOs;
using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Interfaces;
using AlchemistTable.Infrastructure.Data;
using Azure.Core;
using Microsoft.EntityFrameworkCore;

namespace AlchemistTable.Infrastructure.Services
{
    public class PotionService : IPotionService
    {
        private readonly AppDbContext _appDbContext;
        private readonly ILogger<PotionService> _logger;
        public PotionService(AppDbContext appDbContext, ILogger<PotionService> logger)
        {
            _appDbContext = appDbContext ?? throw new ArgumentNullException(nameof(appDbContext));
            _logger = logger;
        }

        public async Task<bool> AddAsync(Potion potion, CancellationToken cancellationToken = default)
        {
            await _appDbContext.Potions.AddAsync(potion, cancellationToken);
            await _appDbContext.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
        {
            var potion = await _appDbContext.Potions.FindAsync(new object[] { id }, cancellationToken);
            if (potion != null)
            {
                _appDbContext.Potions.Remove(potion);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            return false;

        }

        public async Task<IReadOnlyList<Potion>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Potions.ToListAsync(cancellationToken);
        }

        public async Task<Potion?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Potions.FindAsync(new object[] { id }, cancellationToken);
        }

        public async Task<IReadOnlyList<Potion>> GetPotionsByAlchemistAsync(string alchemistId, CancellationToken cancellationToken = default)
        {
            return await _appDbContext.Potions
                .Where(p => p.CreatedBy.ToString() == alchemistId)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> UpdateAsync(Potion potion, CancellationToken cancellationToken = default)
        {
            var existingPotion = await _appDbContext.Potions.FindAsync(new object[] { potion.Id }, cancellationToken);
            if (existingPotion != null)
            {
                existingPotion.Name = potion.Name;
                existingPotion.Effect = potion.Effect;
                existingPotion.EffectPower = potion.EffectPower;
                existingPotion.Ingredients = potion.Ingredients;

                await _appDbContext.SaveChangesAsync(cancellationToken);
                return true;
            }
            else
            {
                throw new KeyNotFoundException($"Potion with ID {potion.Id} not found.");
            }

        }

        public async Task<Potion?> BrewPotion(List<Ingredient> ingredients, CraftPotionRequest request, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation(
                $"Brewing potion {request.Name} by {request.CreatedBy} with {ingredients.Count} ingredients");

            var effect = ingredients.GroupBy(i => i.Effect)
                    .OrderByDescending(g => g.Count())
                    .First().Key;

            var power = ingredients.Count(i => i.Effect == effect);

            var potion = new Potion(request.Name, effect, power, request.CreatedBy, ingredients);
            try
            {
                await _appDbContext.Potions.AddAsync(potion, cancellationToken);
                await _appDbContext.SaveChangesAsync(cancellationToken);
                _logger.LogInformation(
                    $"Potion {potion.Id} created with effect {potion.Effect} (power {potion.EffectPower})");
                return potion;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to brew potion {request.Name}");
                throw new InvalidOperationException("Failed to brew potion. Please try again later.", ex);

            }
        }
    }
}
