using AlchemistTable.Application.DTOs;
using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Interfaces;
using MiniValidation;

namespace AlchemistTable.Application.Handlers
{
    public class BrewPotionHandler
    {
        private readonly IPotionService _potionService;
        private readonly IIngredientService _ingredientService;

        public BrewPotionHandler(IPotionService potionService, IIngredientService ingredientService)
        {
            _potionService = potionService;
            _ingredientService = ingredientService;
        }

        public async Task<(bool Success, string? Error, Potion? Potion)> HandleAsync(CraftPotionRequest request, CancellationToken cancellationToken)
        {
            if (!MiniValidator.TryValidate(request, out var errors))
                return (false, "Invalid request: " + string.Join(", ", errors.SelectMany(e => e.Value)), null);

            var ingredients = (await _ingredientService.GetAllAsync(cancellationToken))
                .Where(i => request.IngredientNames.Contains(i.Name))
                .ToList();

            if (!ingredients.Any())
                return (false, "No valid ingredients provided.", null);

            if (ingredients.Count != request.IngredientNames.Count)
                return (false, "One or more ingredients not found.", null);

            var potion = await _potionService.BrewPotion(ingredients, request, cancellationToken);

            if (potion == null)
                return (false, "Failed to brew potion. Please try again later.", null);

            return (true, null, potion);
        }
    }
}
