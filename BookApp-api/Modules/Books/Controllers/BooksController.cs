using BookApp_api.Modules.Books.Commands;
using BookApp_api.Modules.Books.DTOs;
using BookApp_api.Modules.Books.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApp_api.Modules.Books.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class BooksController(ISender sender) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<List<BookResponse>>> GetAllBooks(CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetAllBooksQuery(), cancellationToken);

            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookResponse>> GetBookById(int id, CancellationToken cancellationToken)
        {
            var result = await sender.Send(new GetBookByIdQuery(id), cancellationToken);

            if (result is null)
            {
                return NotFound(new { message = $"Book with ID {id} was not found." }); ;
            }

            return Ok(result);

        }

        [HttpPost]
        public async Task<ActionResult<BookResponse>> CreateBook(CreateBookRequest request, CancellationToken cancellationToken)
        {
            var command = new AddBookCommand(
                request.Title,
                request.AuthorName,
                request.PublishedDate
            );

            var result = await sender.Send(command, cancellationToken);

            return CreatedAtAction(nameof(GetBookById), new { id = result.BookId }, result);
        }

        [HttpPut("{id:int}")]

        public async Task<IActionResult> UpdateBook(int id, UpdateBookRequest request, CancellationToken cancellationToken)
        {
            var command = new UpdateBookCommand(
                id,
                request.Title,
                request.AuthorName,
                request.PublishedDate);
            await sender.Send(command, cancellationToken);

            return NoContent();
        }

        [HttpDelete("{id:int}")]

        public async Task<IActionResult> DeleteBook(int id, CancellationToken cancellationToken)
        {
            var command = new RemoveBookCommand(id);
            await sender.Send(command, cancellationToken);

            return NoContent();
        }

    }

}
