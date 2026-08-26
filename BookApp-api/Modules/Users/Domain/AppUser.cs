namespace BookApp_api.Modules.Users.Domain;

using BookApp_api.Modules.Quotes.Domain;

public class AppUser
{
    public Guid UserId { get; set; }

    public required string UserName { get; set; }

    public required string PasswordHash { get; set; }

    public DateTime CreatedAtUtc { get; set; }

    public ICollection<Quote> Quotes { get; set; } = new List<Quote>();

}
