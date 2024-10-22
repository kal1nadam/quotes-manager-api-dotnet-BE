using quotesApi.Models;

namespace quotesApi.DTOs.Quotes;

public class CreateQuoteResponse
{
    public QuoteModel Quote { get; set; } = null!;
}