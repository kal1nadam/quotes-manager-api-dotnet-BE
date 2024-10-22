using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using quotesApi.ORM.Entities;

namespace quotesApi.ORM;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser, ApplicationRole, Guid>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Quote> Quotes { get; set; } // Quotes table
    
    public DbSet<QuoteTag> QuoteTags { get; set; } // QuoteTags table
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
    
        modelBuilder.Entity<Quote>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // UserId foreign key - one to many relationship
            // Configure the relationship between Quote and ApplicationUser
            entity.HasOne(q => q.User)
                .WithMany(u => u.Quotes)
                .HasForeignKey(q => q.UserId)
                .IsRequired();
            
        });
        
        modelBuilder.Entity<QuoteTag>(entity =>
        {
            entity.HasKey(e => e.Id);
            
            // QuoteId foreign key - one to many relationship
            // Configure the relationship between QuoteTag and Quote
            entity.HasOne(qt => qt.Quotes)
                .WithMany(q => q.Tags)
                .HasForeignKey(qt => qt.QuoteId)
                .IsRequired();
        });
        
    }
    
}