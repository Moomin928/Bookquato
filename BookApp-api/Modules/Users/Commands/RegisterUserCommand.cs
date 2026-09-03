using BookApp_api.Infrastructure.Data;
using BookApp_api.Extension;
using BookApp_api.Modules.Users.Domain;
using BookApp_api.Modules.Users.DTOs;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Modules.Users.Commands;

public record RegisterUserCommand(string UserName, string Password) : IRequest<AuthResponse>;

public class RegisterUserHandler(
		BookAppDbContext dbContext,
		ITokenService tokenService,
		ILogger<RegisterUserHandler> logger)
		: IRequestHandler<RegisterUserCommand, AuthResponse>
{
	public async Task<AuthResponse> Handle(
			RegisterUserCommand request,
			CancellationToken cancellationToken)
	{
		if (await dbContext.Users.AnyAsync(
						user => user.UserName == request.UserName,
						cancellationToken))
		{
			throw new InvalidOperationException("Username is already in use.");
		}

		var user = new AppUser
		{
			UserId = Guid.NewGuid(),
			UserName = request.UserName,
			PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
			CreatedAtUtc = DateTime.UtcNow
		};

		dbContext.Users.Add(user);
		await dbContext.SaveChangesAsync(cancellationToken);

		logger.LogInformation("Registered user {UserId}", user.UserId);

		return new AuthResponse
		{
			UserId = user.UserId,
			UserName = user.UserName,
			Token = tokenService.CreateToken(user)
		};
	}
}