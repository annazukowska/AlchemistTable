using System.ComponentModel.DataAnnotations;

namespace AlchemistTable.Application.DTOs
{
    public class CraftPotionRequest
    {
        [Required]
        [MinLength(3, ErrorMessage = "Name is required and must be at least 3 characters long")]
        public string Name { get; set; }
        [Required]
        [MinLength(1, ErrorMessage = "Potion requires at least 1 ingredient to be crafted")]
        public List<string> IngredientNames { get; set; } = new List<string>();
        [Required(AllowEmptyStrings = false)]
        public string CreatedBy { get; set; }
    }
}
