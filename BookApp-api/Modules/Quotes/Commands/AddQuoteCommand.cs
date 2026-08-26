using MediatR;
using BookApp_api.Modules.Quotes.DTOs;
using BookApp_api.Infrastructure.Data;
using BookApp_api.Modules.Quotes.Domain;


namespace BookApp_api.Modules.Quotes.Commands;

public record AddQuoteCommand(string Text, string? AuthorName, Guid UserId) : IRequest<QuoteResponse>
{

};

public class AddQuoteCommandHandler(BookAppDbContext dbContext, ILogger<AddQuoteCommandHandler> logger) : IRequestHandler<AddQuoteCommand, QuoteResponse>
{
    public async Task<QuoteResponse> Handle(AddQuoteCommand request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Creating quote for user {UserId}", request.UserId);
        var quote = new Quote
        {
            Text = request.Text,
            AuthorName = request.AuthorName,
            UserId = request.UserId
        };
        dbContext.Quotes.Add(quote);
        await dbContext.SaveChangesAsync(cancellationToken);

        return new QuoteResponse
        {
            QuoteId = quote.QuoteId,
            Text = quote.Text,
            AuthorName = quote.AuthorName,
            CreateAtUtc = quote.CreateAtUtc
        };

    }

}
