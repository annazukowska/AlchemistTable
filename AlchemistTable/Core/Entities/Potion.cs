using AlchemistTable.Core.Enums;

namespace AlchemistTable.Core.Entities
{
    public class Potion : EntityBase
    {
        public string Name { get; set; }
        public Effects Effect { get; set; }
        public int EffectPower { get; set; }
        public List<Ingredient> Ingredients { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedAt { get; init; }

        private Potion() { }
        public Potion(string name, Effects effect, int effectPower, string createdBy, List<Ingredient> ingredients)
        {
            Id = Guid.NewGuid();
            Name = name;
            Effect = effect;
            EffectPower = effectPower;
            CreatedBy = createdBy;
            Ingredients = ingredients ?? new List<Ingredient>();
            CreatedAt = DateTime.UtcNow;
        }
    }
}
