
namespace BookApp_api.Modules.Quotes.DTOs;

public class QuoteResponse
{
    public int QuoteId { get; set; }

    public required string Text { get; set; }

    public string? AuthorName { get; set; }

    public DateTime CreateAtUtc { get; set; }
}
