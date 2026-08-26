using BookApp_api.Modules.Users.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookApp_api.Infrastructure.Data.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
	public void Configure(EntityTypeBuilder<AppUser> builder)
	{
		builder.HasKey(user => user.UserId);

		builder.Property(user => user.UserName)
				.IsRequired()
				.HasMaxLength(50);

		builder.HasIndex(user => user.UserName)
				.IsUnique();

		builder.Property(user => user.PasswordHash)
				.IsRequired();

		builder.Property(user => user.CreatedAtUtc)
				.IsRequired();

		builder.HasMany(user => user.Quotes)
				.WithOne(quote => quote.User)
				.HasForeignKey(quote => quote.UserId)
				.OnDelete(DeleteBehavior.Cascade);
	}
}