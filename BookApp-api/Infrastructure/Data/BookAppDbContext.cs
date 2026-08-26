using BookApp_api.Modules.Books.Domain;
using Microsoft.EntityFrameworkCore;
using BookApp_api.Modules.Quotes.Domain;
using BookApp_api.Modules.Users.Domain;

namespace BookApp_api.Infrastructure.Data;

public class BookAppDbContext : DbContext
{
    public BookAppDbContext(DbContextOptions options) : base(options)
    {

    }
    public DbSet<Book> Books { get; set; }
    public DbSet<Quote> Quotes { get; set; }
    public DbSet<AppUser> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookAppDbContext).Assembly);
    }
}
