using System.ComponentModel.DataAnnotations;

namespace BookApp_api.Modules.Books.DTOs;

public class CreateBookRequest
{
    [Required]
    [MaxLength(200)]
    public required string Title { get; set; }

    [Required]
    [MaxLength(150)]
    public required string AuthorName { get; set; }


    public DateOnly? PublishedDate { get; set; }
}
