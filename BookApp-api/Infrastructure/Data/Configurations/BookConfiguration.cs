using BookApp_api.Modules.Books.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BookApp_api.Infrastructure.Data.Configurations;

public class BookConfiguration : IEntityTypeConfiguration<Book>
{
    public void Configure(EntityTypeBuilder<Book> builder)
    {
        builder.HasKey(book => book.BookId);

        builder.Property(book => book.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(book => book.AuthorName)
            .IsRequired()
            .HasMaxLength(150);

        builder.Property(book => book.PublishedDate)
            .IsRequired(false);
    }
}
