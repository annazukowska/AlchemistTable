using AlchemistTable.Core.Enums;

namespace AlchemistTable.Core.Entities
{
    public class Ingredient : EntityBase
    {
        public string Name { get; set; }
        public IngredientRarity Rarity { get; set; }
        public Effects Effect { get; set; }
        public string Description { get; set; }

        public Ingredient() { }
        public Ingredient(string name, IngredientRarity rarity, Effects effect, string? description)
        {
            Id = Guid.NewGuid();
            Name = name;
            Rarity = rarity;
            Effect = effect;
            Description = description ?? "No description available.";
        }
    }

}
