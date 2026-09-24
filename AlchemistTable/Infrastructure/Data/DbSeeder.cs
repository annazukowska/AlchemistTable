using AlchemistTable.Core.Entities;
using AlchemistTable.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace AlchemistTable.Infrastructure.Data
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            if (!context.Ingredients.Any())
            {
                var ingredients = new List<Ingredient>
                {
                    new("Fire Essence", IngredientRarity.Common, Effects.Burn, "A volatile fragment of flame captured in crystal form."),
                    new("Ice Crystal", IngredientRarity.Uncommon, Effects.Freeze, "A chilling shard that radiates cold energy."),
                    new("Shadow Dust", IngredientRarity.Rare, Effects.Invisibility, "Fine powder collected from shadow beasts, bends light."),
                    new("Sunroot", IngredientRarity.Common, Effects.Healing, "A radiant root known for its healing properties."),
                    new("Thunderleaf", IngredientRarity.Rare, Effects.Stun, "Electrified plant that can jolt enemies into paralysis."),
                    new("Ghoul Mushroom", IngredientRarity.Common, Effects.Poison, "Fungi that grow on undead remains, toxic and deadly."),
                    new("Dragon Scale", IngredientRarity.Legendary, Effects.FireResistance, "Ancient scale resistant to the hottest flames."),
                    new("Moonflower", IngredientRarity.Uncommon, Effects.ManaRegen, "A glowing bloom that restores magical energy."),
                    new("Spirit Ash", IngredientRarity.Mythic, Effects.Revive, "Sacred ash said to bring back life from death.")
                };
                await context.Ingredients.AddRangeAsync(ingredients);
            }

            if (!context.Alchemists.Any())
            {
                var ingredients = context.Ingredients.Take(6).ToList();

                var alchemists = new List<Alchemist>
                {
                    new("Elrik","elrik@potions.com","test1234", ingredients.Take(3).ToList()),
                    new("Mira", "mira@potions.com", "test1234",ingredients.Skip(1).Take(3).ToList()),
                    new("Thorne","thorne@potions.com","test1234", ingredients.Skip(2).Take(3).ToList()),
                    new("Isla", "isla@potions.com", "test1234", ingredients.Skip(3).Take(3).ToList()),
                    new("Steffen", "steffen@potions.com", "mamusia1234", ingredients.Skip(4).Take(3).ToList()),
                    new("Bram", "bram@potions.com", "test1234", ingredients.Skip(5).Take(3).ToList()),
                    new("Lira", "lira@potions.com", "test1234", ingredients.Skip(1).Take(3).ToList())
                };
                await context.Alchemists.AddRangeAsync(alchemists);
            }

            if (!context.Potions.Any())
            {
                var alchemists = context.Alchemists.Include(a => a.Inventory).ToList();
                var ingredients = context.Ingredients.Take(6).ToList();

                var potions = new List<Potion>();

                for (int i = 0; i < 10; i++)
                {
                    var selectedIngredients = ingredients.OrderBy(_ => Guid.NewGuid()).Take(3).ToList();
                    var effect = selectedIngredients.GroupBy(i => i.Effect).OrderByDescending(g => g.Count()).First().Key;
                    var power = selectedIngredients.Count(i => i.Effect == effect);
                    var alchemist = alchemists[i % alchemists.Count];

                    potions.Add(new Potion($"Potion {i + 1}", effect, power, alchemist.Id.ToString(), selectedIngredients));
                }

                await context.Potions.AddRangeAsync(potions);
            }

            await context.SaveChangesAsync();
        }
    }
}
