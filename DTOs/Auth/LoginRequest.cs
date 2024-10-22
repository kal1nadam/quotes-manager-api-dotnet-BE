using System.ComponentModel.DataAnnotations;

namespace quotesApi.DTOs.Auth;

public class LoginRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
}