

using System.ComponentModel.DataAnnotations;

namespace BookApp_api.Modules.Quotes.DTOs;

public class CreateQuoteRequest
{
    [Required]
    [MaxLength(2000)]
    public required string Text { get; set; }

    [MaxLength(150)]
    public string? AuthorName { get; set; }

}
