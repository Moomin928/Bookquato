using System.Security.Claims;
using BookApp_api.Modules.Quotes.Commands;
using BookApp_api.Modules.Quotes.DTOs;
using BookApp_api.Modules.Quotes.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BookApp_api.Modules.Quotes.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class QuotesController(ISender sender) : ControllerBase
{
    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedAccessException();
        }

        return userId;
    }

    [HttpGet]
    public async Task<ActionResult<List<QuoteResponse>>> GetAllQuotes(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await sender.Send(
            new GetAllQuotesQuery(userId),
            cancellationToken);

        return Ok(result);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<QuoteResponse>> GetQuoteById(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await sender.Send(
            new GetQuoteByIdQuery(id, userId),
            cancellationToken);

        return result is null ? NotFound() : Ok(result);
    }

    [HttpPost]
    public async Task<ActionResult<QuoteResponse>> CreateQuote(
        CreateQuoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var command = new AddQuoteCommand(request.Text, request.AuthorName, userId);
        var result = await sender.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetQuoteById), new { id = result.QuoteId }, result);

    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateQuote(
        int id,
        UpdateQuoteRequest request,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var command = new UpdateQuoteCommand(id, request.Text, request.AuthorName, userId);
        await sender.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteQuote(
        int id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var command = new RemoveQuoteCommand(id, userId);
        await sender.Send(command, cancellationToken);
        return NoContent();
    }
}