
using BookApp_api.Infrastructure.Data;
using BookApp_api.Modules.Books.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Books.Queries;

public class GetAllBooksQuery : IRequest<List<BookResponse>>
{

};
public class GetAllBooksHandler(BookAppDbContext dbContext, ILogger<GetAllBooksHandler> logger) : IRequestHandler<GetAllBooksQuery, List<BookResponse>>
{
    public async Task<List<BookResponse>> Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        logger.LogInformation("Fetching books");

        return await dbContext.Books
            .AsNoTracking()
            .Select(book => new BookResponse
            {
                BookId = book.BookId,
                Title = book.Title,
                AuthorName = book.AuthorName,
                PublishedDate = book.PublishedDate
            })
            .ToListAsync(cancellationToken);
    }
}