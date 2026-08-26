
using BookApp_api.Modules.Users.Domain;

namespace BookApp_api.Modules.Quotes.Domain;

public class Quote
{
    public int QuoteId { get; set; }

    public required string Text { get; set; }

    public string? AuthorName { get; set; }

    public Guid UserId { get; set; }

    public AppUser? User { get; set; }

    public DateTime CreateAtUtc { get; set; } = DateTime.UtcNow;
}
