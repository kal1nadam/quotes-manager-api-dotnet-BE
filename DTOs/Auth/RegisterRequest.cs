using System.ComponentModel.DataAnnotations;

namespace quotesApi.DTOs.Auth;

public class RegisterRequest
{
    [EmailAddress]
    [Required]
    public string Email { get; set; } = null!;
    
    [Required]
    public string Password { get; set; } = null!;
    
    [Compare(nameof(Password))]
    [Required]
    public string ConfirmPassword { get; set; } = null!;
}