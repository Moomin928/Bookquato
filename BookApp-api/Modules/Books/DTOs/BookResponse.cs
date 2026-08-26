namespace BookApp_api.Modules.Books.DTOs;

public class BookResponse
{
    public int BookId { get; set; }

    public required string Title { get; set; }

    public required string AuthorName { get; set; }

    public DateOnly? PublishedDate { get; set; }
}
