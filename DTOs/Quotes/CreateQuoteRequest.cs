using System.ComponentModel.DataAnnotations;
using quotesApi.Enums;

namespace quotesApi.DTOs.Quotes;

public class CreateQuoteRequest
{
    [Required]
    public string Text { get; set; } = null!;
    [Required]
    public string Author { get; set; } = null!;
    public List<QuoteTagType> Tags { get; set; } = new();
}