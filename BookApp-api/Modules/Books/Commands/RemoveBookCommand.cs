using BookApp_api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Books.Commands;

public record RemoveBookCommand(int BookId) : IRequest<Unit>;

public class RemoveBookHandler(BookAppDbContext dbContext, ILogger<RemoveBookHandler> logger) : IRequestHandler<RemoveBookCommand, Unit>
{
    public async Task<Unit> Handle(RemoveBookCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Removing book with ID:{BookId}", request.BookId);
        var book = await dbContext.Books
            .FirstOrDefaultAsync(book => book.BookId == request.BookId, cancellationToken);

        if (book is null)
        {
            throw new KeyNotFoundException($"Book with ID {request.BookId} was not found.");
        }
        dbContext.Books.Remove(book);

        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;
    }
}
