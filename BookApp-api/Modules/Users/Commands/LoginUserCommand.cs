using BookApp_api.Infrastructure.Data;
using BookApp_api.Extension;
using BookApp_api.Modules.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Users.Commands;

public record LoginUserCommand(string UserName, string Password) : IRequest<AuthResponse>;

public class LoginUserHandler(
		BookAppDbContext dbContext,
		ITokenService tokenService,
		ILogger<LoginUserHandler> logger)
		: IRequestHandler<LoginUserCommand, AuthResponse>
{
	public async Task<AuthResponse> Handle(
			LoginUserCommand request,
			CancellationToken cancellationToken)
	{
		var user = await dbContext.Users
				.FirstOrDefaultAsync(
						candidate => candidate.UserName == request.UserName,
						cancellationToken);

		if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
		{
			throw new UnauthorizedAccessException("Invalid username or password.");
		}

		logger.LogInformation("User {UserId} logged in", user.UserId);

		return new AuthResponse
		{
			UserId = user.UserId,
			UserName = user.UserName,
			Token = tokenService.CreateToken(user)
		};
	}
}