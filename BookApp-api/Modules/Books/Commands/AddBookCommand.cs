using MediatR;
using BookApp_api.Modules.Books.DTOs;
using BookApp_api.Infrastructure.Data;
using BookApp_api.Modules.Books.Domain;

namespace BookApp_api.Modules.Books.Commands;

public record AddBookCommand
(
    string Title,
    string AuthorName,
    DateOnly? PublishedDate

) : IRequest<BookResponse>;

public class AddBookHandler(BookAppDbContext dbContext, ILogger<AddBookHandler> logger) : IRequestHandler<AddBookCommand, BookResponse>
{
    public async Task<BookResponse> Handle(AddBookCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Create a new Book:{Title}", request.Title);

        var book = new Book
        {
            Title = request.Title,
            AuthorName = request.AuthorName,
            PublishedDate = request.PublishedDate
        };
        dbContext.Books.Add(book);

        await dbContext.SaveChangesAsync(cancellationToken);

        return new BookResponse
        {
            BookId = book.BookId,
            Title = book.Title,
            AuthorName = book.AuthorName,
            PublishedDate = book.PublishedDate
        };

    }
}
