using quotesApi.Enums;

namespace quotesApi.Models;

public class QuoteModel
{
    public Guid Id { get; set; }
    public string Text { get; set; } = null!;
    public string Author { get; set; } = null!;
    public List<QuoteTagType> Tags { get; set; } = new();
}