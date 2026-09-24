using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Enums;
using AlchemistTable.Core.Interfaces;

namespace AlchemistTable.Infrastructure.Services.InMemory
{
    public class IngredientServiceInMemory : IIngredientService
    {
        private readonly List<Ingredient> _ingredients = new()
            {
                new("Mandrake Root", IngredientRarity.Common, Effects.Healing, "A twisted root known for its restorative properties and eerie scream."),
                new("Dragon Scale", IngredientRarity.Rare, Effects.FireResistance, "A crimson scale radiating heat, shed from an ancient fire drake."),
                new("Nightshade Berry", IngredientRarity.Uncommon, Effects.Poison, "Glossy purple berries that can either numb pain or end a life."),
                new("Mermaid Tears", IngredientRarity.Rare, Effects.WaterBreathing, "Collected under moonlight, these crystalline droplets hold the essence of the sea."),
                new("Griffin Feather", IngredientRarity.Rare, Effects.Agility, "Light as air, a single feather from a skyborne predator."),
                new("Troll Fat", IngredientRarity.Common, Effects.Defense, "Thick, greenish fat rendered from a cave troll, known for its toughness."),
                new("Phoenix Ashes", IngredientRarity.VeryRare, Effects.Resistance, "Glowing embers that pulse with life, gathered after rebirth."),
                new("Elf Hair", IngredientRarity.Uncommon, Effects.Intelligence, "Silver strands often used in clarity-enhancing brews."),
                new("Vampire Blood", IngredientRarity.VeryRare, Effects.Damage, "A dark vial containing stolen power and raw aggression."),
                new("Will-o'-Wisp Essence", IngredientRarity.Rare, Effects.Speed, "Captured from wandering spirits in swamps, gives an unnatural burst of speed."),
                new("Stonecap Mushroom", IngredientRarity.Common, Effects.Defense, "Grows near enchanted rocks; hardens skin when brewed."),
                new("Basilisk Eye Dust", IngredientRarity.Legendary, Effects.Poison, "A deadly powder ground from petrified eyes."),
                new("Nimbus Leaf", IngredientRarity.Uncommon, Effects.Intelligence, "A floating leaf that crackles faintly with arcane static."),
                new("Minotaur Horn Shard", IngredientRarity.Rare, Effects.Strength, "Heavy and coarse, it holds raw brute force."),
                new("Astral Bloom", IngredientRarity.Legendary, Effects.Healing, "A radiant flower that only grows once every hundred years during a lunar eclipse.")
            };

        public Task<IReadOnlyList<Ingredient>> GetAllAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Ingredient>>(_ingredients);

        public Task<Ingredient?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(_ingredients.FirstOrDefault(i => i.Id == id));

        public Task<IReadOnlyList<Ingredient>> GetIngredientsByNameAsync(string name, CancellationToken cancellationToken)
        {
            var ingredients = _ingredients.Where(i => i.Name.Contains(name, StringComparison.OrdinalIgnoreCase)).ToList();
            IReadOnlyList<Ingredient> result = ingredients;
            return Task.FromResult(result);
        }

        public Task<bool> UpdateAsync(Ingredient ingredient, CancellationToken cancellationToken) 
        {
            var existing = _ingredients.FirstOrDefault(i => i.Id == ingredient.Id);
            if (existing != null)
            {
                existing.Name = ingredient.Name;
                existing.Rarity = ingredient.Rarity;
                existing.Effect = ingredient.Effect;
                existing.Description = ingredient.Description;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
        public Task<bool> AddAsync(Ingredient ingredient, CancellationToken cancellationToken)
        {
            _ingredients.Add(ingredient);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var ingredient = _ingredients.FirstOrDefault(i => i.Id == id);
            if (ingredient != null)
            {
                _ingredients.Remove(ingredient);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }
    }
}
