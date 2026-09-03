using BookApp_api.Modules.Users.Commands;
using BookApp_api.Modules.Users.DTOs;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BookApp_api.Modules.Users.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(ISender sender) : ControllerBase
{
	[HttpPost("register")]
	public async Task<ActionResult<AuthResponse>> Register(
			RegisterUserRequest request,
			CancellationToken cancellationToken)
	{
		try
		{
			var result = await sender.Send(
					new RegisterUserCommand(request.UserName, request.Password),
					cancellationToken);

			return StatusCode(StatusCodes.Status201Created, result);
		}
		catch (InvalidOperationException exception)
		{
			return Conflict(new { message = exception.Message });
		}
	}

	[HttpPost("login")]
	public async Task<ActionResult<AuthResponse>> Login(
			LoginUserRequest request,
			CancellationToken cancellationToken)
	{
		try
		{
			return Ok(await sender.Send(
					new LoginUserCommand(request.UserName, request.Password),
					cancellationToken));
		}
		catch (UnauthorizedAccessException)
		{
			return Unauthorized(new { message = "Invalid username or password." });
		}
	}
}