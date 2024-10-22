namespace quotesApi.DTOs.Auth;

public class RegisterResponse
{
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
}