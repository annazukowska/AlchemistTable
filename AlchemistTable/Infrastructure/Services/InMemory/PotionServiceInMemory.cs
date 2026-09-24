using AlchemistTable.Application.DTOs;
using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Enums;
using AlchemistTable.Core.Interfaces;

namespace AlchemistTable.Infrastructure.Services.InMemory
{
    public class PotionServiceInMemory : IPotionService
    {
        private readonly List<Potion> _potions = new ()
        {
            new Potion("Lesser Healing Elixir", Effects.Healing, 1, new Guid("00000000-0000-0000-0000-000000000001").ToString(),
                new List<Ingredient>
                {
                    new("Mandrake Root", IngredientRarity.Common, Effects.Healing, "A twisted root..."),
                    new("Stonecap Mushroom", IngredientRarity.Common, Effects.Defense, "Grows near rocks...")
                }
            ),
            new Potion ("Potion of Fire Ward", Effects.FireResistance, 2, new Guid("00000000-0000-0000-0000-000000000002").ToString(),
                new List<Ingredient>
                {
                    new("Dragon Scale", IngredientRarity.Rare, Effects.FireResistance, "A crimson scale..."),
                    new("Phoenix Ashes", IngredientRarity.VeryRare, Effects.Resistance, "Glowing embers...")
                }
               ),
            new Potion ("Venom Draught", Effects.Poison, 3, new Guid("00000000-0000-0000-0000-000000000003").ToString(),
                new List<Ingredient>
                {
                    new("Basilisk Eye Dust", IngredientRarity.Legendary, Effects.Poison, "Petrified eyes..."),
                    new("Nightshade Berry", IngredientRarity.Uncommon, Effects.Poison, "Glossy purple berries...")
                }
            )
        };

        public Task<IReadOnlyList<Potion>> GetAllAsync(CancellationToken cancellationToken) => Task.FromResult<IReadOnlyList<Potion>>(_potions);

        public Task<Potion?> GetByIdAsync(Guid id, CancellationToken cancellationToken) => Task.FromResult(_potions.FirstOrDefault(i => i.Id == id));

        public Task<bool> AddAsync(Potion potion, CancellationToken cancellationToken) 
        {
            _potions.Add(potion);
            return Task.FromResult(true);
        }

        public Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken)
        {
            var potion = _potions.FirstOrDefault(p => p.Id == id);
            if (potion != null)
            {
                _potions.Remove(potion);
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<bool> UpdateAsync(Potion potion, CancellationToken cancellationToken)
        {
            var potionIndex = _potions.FindIndex(p => p.Id == potion.Id);
            if (potionIndex != -1)
            {
                _potions[potionIndex].Name = potion.Name;
                _potions[potionIndex].Effect = potion.Effect;
                _potions[potionIndex].EffectPower = potion.EffectPower;
                _potions[potionIndex].Ingredients = potion.Ingredients;
                return Task.FromResult(true);
            }
            return Task.FromResult(false);
        }

        public Task<Potion?> BrewPotion(List<Ingredient> ingredients, CraftPotionRequest request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<Potion>> GetPotionsByAlchemistAsync(string alchemistId, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();  
        }
    }
}
