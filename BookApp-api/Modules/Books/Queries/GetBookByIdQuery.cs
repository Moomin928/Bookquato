using BookApp_api.Infrastructure.Data;
using BookApp_api.Modules.Books.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Books.Queries;

public class GetBookByIdQuery(int bookId) : IRequest<BookResponse?>
{
    public int BookId { get; } = bookId;
}

public class GetBookByIdHandler(
    BookAppDbContext dbContext,
    ILogger<GetBookByIdHandler> logger)
    : IRequestHandler<GetBookByIdQuery, BookResponse?>
{
    public async Task<BookResponse?> Handle(
        GetBookByIdQuery request,
        CancellationToken cancellationToken)
    {
        logger.LogInformation(
            "Fetching the book by ID: {BookId}",
            request.BookId);

        return await dbContext.Books
            .AsNoTracking()
            .Where(book => book.BookId == request.BookId)
            .Select(book => new BookResponse
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorName = book.AuthorName,
                PublishedDate = book.PublishedDate
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}