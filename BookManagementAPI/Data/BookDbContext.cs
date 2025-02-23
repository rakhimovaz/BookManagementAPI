using Microsoft.EntityFrameworkCore;
using BookManagementAPI.Models;

namespace BookManagementAPI.Data;

public class BookDbContext : DbContext
{
    public BookDbContext(DbContextOptions<BookDbContext> options)
        : base(options)
    {
    }

    public DbSet<Book> Books { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasIndex(e => e.Title).IsUnique();
            
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Global query filter to exclude soft-deleted books
            entity.HasQueryFilter(b => !b.IsDeleted);
        });
    }

    // Method to get all books including deleted ones
    public IQueryable<Book> GetBooksIncludingDeleted()
    {
        return Set<Book>().IgnoreQueryFilters();
    }
}
