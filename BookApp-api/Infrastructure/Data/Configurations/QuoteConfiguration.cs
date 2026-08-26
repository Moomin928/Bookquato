using BookApp_api.Modules.Quotes.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookApp_api.Infrastructure.Data.Configurations;

public class QuoteConfiguration : IEntityTypeConfiguration<Quote>
{
	public void Configure(EntityTypeBuilder<Quote> builder)
	{
		builder.HasKey(quote => quote.QuoteId);

		builder.Property(quote => quote.Text)
				.IsRequired()
				.HasMaxLength(2000);

		builder.Property(quote => quote.AuthorName)
				.HasMaxLength(150);

		builder.Property(quote => quote.UserId)
				.IsRequired();

		builder.Property(quote => quote.CreateAtUtc)
				.IsRequired();
	}
}