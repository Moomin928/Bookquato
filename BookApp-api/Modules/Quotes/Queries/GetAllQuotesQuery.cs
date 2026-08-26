using BookApp_api.Infrastructure.Data;
using BookApp_api.Modules.Quotes.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Quotes.Queries;

public record GetAllQuotesQuery(Guid UserId) : IRequest<List<QuoteResponse>>
{

};

public class GetAllQuotesHandler(BookAppDbContext dbContext, ILogger<GetAllQuotesHandler> logger) : IRequestHandler<GetAllQuotesQuery, List<QuoteResponse>>
{
    public async Task<List<QuoteResponse>> Handle(GetAllQuotesQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching quotes for the User {UserId}", request.UserId);

        return await dbContext.Quotes
            .AsNoTracking()
            .Where(quote => quote.UserId == request.UserId)
            .Select(quote => new QuoteResponse
            {
                QuoteId = quote.QuoteId,
                Text = quote.Text,
                AuthorName = quote.AuthorName,
                CreateAtUtc = quote.CreateAtUtc
            })
            .ToListAsync(cancellationToken);
    }

}
