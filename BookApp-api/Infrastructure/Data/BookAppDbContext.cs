using BookApp_api.Modules.Books.Domain;
using Microsoft.EntityFrameworkCore;

namespace BookApp_api.Infrastructure.Data;

public class BookAppDbContext : DbContext
{
    public BookAppDbContext(DbContextOptions options) : base(options)
    {

    }
    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BookAppDbContext).Assembly);
    }
}
