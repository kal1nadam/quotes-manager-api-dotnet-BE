using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using quotesApi.ORM.Entities;

namespace quotesApi.Controllers;

[ApiController]
[Route("[controller]/[action]")]
public class TestController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;

    public TestController(UserManager<ApplicationUser> userManager, RoleManager<ApplicationRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }
    
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> AssignAdminRole()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        // assign role "admin" to User
        var user = await _userManager.FindByIdAsync(userId);

        await _roleManager.CreateAsync(new ApplicationRole { Name = "admin" });
        
        await _userManager.AddToRoleAsync(user, "admin");
        

        return Ok();
    }
}