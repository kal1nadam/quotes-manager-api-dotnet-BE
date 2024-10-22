using quotesApi.Enums;

namespace quotesApi.ORM.Entities;

public class QuoteTag
{
    public Guid Id { get; set; }
    public Guid QuoteId { get; set; }
    public QuoteTagType Type { get; set; }

    public virtual Quote Quotes { get; set; } 
}