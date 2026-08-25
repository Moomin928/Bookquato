namespace BookApp_api.Modules.Books.Domain;

public class Book
{
    public int BookId { get; set; }

    public required string Title { get; set; }

    public required string AuthorName { get; set; }

    public DateOnly? PublishedDate { get; set; }

}
