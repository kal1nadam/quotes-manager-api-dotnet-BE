using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quotesApi.DTOs.Auth;
using quotesApi.ORM.Entities;
using quotesApi.Services;

namespace quotesApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    
    private readonly JwtTokenService _jwtTokenService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        JwtTokenService jwtTokenService)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _jwtTokenService = jwtTokenService;
    }
    
    
    [HttpPost]
    public async Task<ActionResult<RegisterResponse>> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            var refreshToken = await _jwtTokenService.GenerateRefreshToken(user);
            var accessToken = await _jwtTokenService.GenerateAccessToken(user);
            
            // Save the refresh token to the user
            user.RefreshToken = refreshToken;
            await _userManager.UpdateAsync(user);
            
            return new RegisterResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken
            };
        }
        
        foreach (var error in result.Errors)
            ModelState.AddModelError(string.Empty, error.Description);

        return BadRequest(ModelState);
    }
    
    
    [HttpPost]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);

        if (user == null)
        {
            return Unauthorized("Invalid email or password");
        }

        var result = await _signInManager.PasswordSignInAsync(user, request.Password, false, false);

        if (!result.Succeeded)
        {
            return Unauthorized("Invalid email or password");
        }

        var refreshToken = await _jwtTokenService.GenerateRefreshToken(user);
        var accessToken = await _jwtTokenService.GenerateAccessToken(user);
            
        // Save the refresh token to the user
        user.RefreshToken = refreshToken;
        await _userManager.UpdateAsync(user);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        });
    }
    
    [HttpPost]
    public async Task<ActionResult<GetAccessTokenResponse>> GetAccessToken()
    {
        
        var refreshToken = Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

        if (string.IsNullOrEmpty(refreshToken))
        {
            return Unauthorized("Refresh token is missing");
        }
        
        // unwrap the jwt token and retrieve the user id
        
        var principal = _jwtTokenService.GetTokenClaims(refreshToken);

        var userEmail = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
        if (userEmail == null)
        {
            return Unauthorized("Invalid refresh token - no email claim");
        }
        
        var expirationDateUnix = principal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Exp)?.Value;
        if (expirationDateUnix == null)
        {
            return Unauthorized("Invalid refresh token - no email claim");
        }

        // Find the user in the database by email
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null)
        {
            return Unauthorized("Invalid user");
        }

        
        if (user.RefreshToken != refreshToken)
        {
            return BadRequest("Invalid refresh token"); //400
        }
        
        
        var exp = DateTimeOffset.FromUnixTimeSeconds(long.Parse(expirationDateUnix)).UtcDateTime;
        // Check if the token is expired
        if (exp < DateTime.UtcNow)
        {
            return BadRequest("Invalid refresh token");
        }
        
        var response = new GetAccessTokenResponse()
        {
            AccessToken = await _jwtTokenService.GenerateAccessToken(user)
        };

        return Ok(response);
    }
}