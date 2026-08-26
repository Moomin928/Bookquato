using BookApp_api.Infrastructure.Data;
using BookApp_api.Modules.Quotes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Quotes.Queries;

public record GetQuoteByIdQuery(int QuoteId, Guid UserId) : IRequest<QuoteResponse?>
{

}

public class GetQuoteByIdHandler(BookAppDbContext dbContext, ILogger<GetQuoteByIdHandler> logger) : IRequestHandler<GetQuoteByIdQuery, QuoteResponse?>
{
    public async Task<QuoteResponse?> Handle(GetQuoteByIdQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation(
              "Fetching quote {QuoteId} for user {UserId}",
              request.QuoteId,
              request.UserId);
        return await dbContext.Quotes
            .AsNoTracking()
            .Where(quote => quote.QuoteId == request.QuoteId && quote.UserId == request.UserId)
            .Select(quote => new QuoteResponse
            {
                QuoteId = quote.QuoteId,
                Text = quote.Text,
                AuthorName = quote.AuthorName,
                CreateAtUtc = quote.CreateAtUtc
            })
            .FirstOrDefaultAsync(cancellationToken);
    }
}
