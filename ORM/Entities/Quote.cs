namespace quotesApi.ORM.Entities;

public class Quote
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    public string Author { get; set; } = null!;
    public Guid UserId { get; set; }
    
    public virtual ApplicationUser User { get; set; } = null!;
    
    public virtual ICollection<QuoteTag> Tags { get; set; } = new List<QuoteTag>();
}