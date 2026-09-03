using BookApp_api.Infrastructure.Data;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Quotes.Commands
{
    public record RemoveQuoteCommand(int QuoteId, Guid UserId) : IRequest<Unit>
    {

    }
    public class RemoveQuoteHandler(BookAppDbContext dbContext, ILogger<RemoveQuoteHandler> logger) : IRequestHandler<RemoveQuoteCommand, Unit>
    {
        public async Task<Unit> Handle(RemoveQuoteCommand request, CancellationToken cancellationToken)
        {
            logger.LogInformation("Removing quote {QuoteId} for {UserId}", request.QuoteId, request.UserId);
            var quote = await dbContext.Quotes
                .FirstOrDefaultAsync(quote => quote.QuoteId == request.QuoteId && quote.UserId == request.UserId, cancellationToken);
            if (quote is null)
            {
                throw new KeyNotFoundException($"Quote with ID {request.QuoteId} was not found.");
            }
            dbContext.Quotes.Remove(quote);
            await dbContext.SaveChangesAsync(cancellationToken);
            return Unit.Value;
        }
    }

}