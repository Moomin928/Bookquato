using BookApp_api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Books.Commands;

public record UpdateBookCommand(
    int BookId,
    string Title,
    string AuthorName,
    DateOnly? PublishedDate
) : IRequest<Unit>;

public class UpdateBookHandler(BookAppDbContext dbContext, ILogger<UpdateBookHandler> logger) : IRequestHandler<UpdateBookCommand, Unit>
{
    public async Task<Unit> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Update the book with {BookId}", request.BookId);
        var book = await dbContext.Books
            .FirstOrDefaultAsync(book => book.BookId == request.BookId, cancellationToken);

        if (book is null)
        {
            throw new KeyNotFoundException(
                $"Book with ID {request.BookId} was not found.");

        }

        book.Title = request.Title;
        book.AuthorName = request.AuthorName;
        book.PublishedDate = request.PublishedDate;

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }
}