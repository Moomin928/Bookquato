using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using BookApp_api.Modules.Users.Domain;
using Microsoft.IdentityModel.Tokens;

namespace BookApp_api.Extension;

public interface ITokenService
{
	string CreateToken(AppUser user);
}

public class TokenService(IConfiguration configuration) : ITokenService
{
	public string CreateToken(AppUser user)
	{
		var jwt = configuration.GetSection("Jwt");
		var key = jwt["Key"] ?? throw new InvalidOperationException("JWT key is not configured.");
		var issuer = jwt["Issuer"] ?? throw new InvalidOperationException("JWT issuer is not configured.");
		var audience = jwt["Audience"] ?? throw new InvalidOperationException("JWT audience is not configured.");
		var expiryMinutes = jwt.GetValue<int>("ExpiryMinutes");

		var credentials = new SigningCredentials(
				new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)),
				SecurityAlgorithms.HmacSha256);

		var claims = new[]
		{
						new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
						new Claim(ClaimTypes.Name, user.UserName)
				};

		var token = new JwtSecurityToken(
				issuer,
				audience,
				claims,
				expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
				signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}