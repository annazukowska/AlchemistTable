using System.ComponentModel.DataAnnotations;

namespace AlchemistTable.Application.DTOs
{

    public class RefreshTokenRequest
    {
        [Required(ErrorMessage = "Refresh token is required")]
        public string RefreshToken { get; init; }
    }
}
