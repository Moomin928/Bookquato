namespace BookApp_api.Modules.Users.DTOs;

public class AuthResponse
{
	public required Guid UserId { get; set; }

	public required string UserName { get; set; }

	public required string Token { get; set; }
}