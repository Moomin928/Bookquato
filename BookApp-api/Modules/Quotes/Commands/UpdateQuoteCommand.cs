using BookApp_api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Quotes.Commands;

public record UpdateQuoteCommand(
    int QuoteId,
    string Text,
    string? AuthorName,
    Guid UserId
) : IRequest<Unit>;

public class UpdateQuoteHandler(BookAppDbContext dbContext, ILogger<UpdateQuoteHandler> logger) : IRequestHandler<UpdateQuoteCommand, Unit>
{
    public async Task<Unit> Handle(UpdateQuoteCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Update the quote {QuoteId} with {UserId}", request.QuoteId, request.UserId);
        var quote = await dbContext.Quotes
            .FirstOrDefaultAsync(quote => quote.QuoteId == request.QuoteId && quote.UserId == request.UserId, cancellationToken);

        if (quote is null)
        {
            throw new KeyNotFoundException(
                $"Quote {request.QuoteId} not found for user {request.UserId}.");

        }

        quote.Text = request.Text;
        quote.AuthorName = request.AuthorName;


        await dbContext.SaveChangesAsync(cancellationToken);

        return Unit.Value;

    }
}