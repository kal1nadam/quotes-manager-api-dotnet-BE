using quotesApi.Enums;
using quotesApi.Models;

namespace quotesApi.DTOs.Quotes;

public class GetQuotesResponse
{
    public List<QuoteModel> Quotes { get; set; } = null!;
}

