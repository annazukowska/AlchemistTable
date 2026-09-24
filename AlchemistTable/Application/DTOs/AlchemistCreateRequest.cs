using System.ComponentModel.DataAnnotations;
using AlchemistTable.Core.Entities;

namespace AlchemistTable.Application.DTOs
{
    public class AlchemistCreateRequest
    {
        [Required(ErrorMessage = "Name is required")]
        [MinLength(3, ErrorMessage = "Name must be at least 3 characters long")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [Display(Name = "Contact Rune")] // Thematic replacement for Email/Login
        public string Email { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [Display(Name = "Secret Code")] // Thematic replacement for Password
        public string Password { get; set; }

        public List<Ingredient> Inventory { get; set; } = new();
    }
}
