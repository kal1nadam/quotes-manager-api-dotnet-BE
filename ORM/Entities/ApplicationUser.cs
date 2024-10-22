using Microsoft.AspNetCore.Identity;

namespace quotesApi.ORM.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? RefreshToken { get; set; }
    
    public virtual ICollection<Quote> Quotes { get; set; } = null!;
}