using System.ComponentModel.DataAnnotations;

namespace BookApp_api.Modules.Users.DTOs;

public class RegisterUserRequest
{
	[Required]
	[MaxLength(50)]
	public required string UserName { get; set; }

	[Required]
	public required string Password { get; set; }
}