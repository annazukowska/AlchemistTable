using System.ComponentModel.DataAnnotations;

namespace AlchemistTable.Application.DTOs
{
    public class AlchemistLoginRequest
    {
        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public AlchemistLoginRequest(string email, string password)
        {
            Email = email;
            Password = password;
        }
    }
}
