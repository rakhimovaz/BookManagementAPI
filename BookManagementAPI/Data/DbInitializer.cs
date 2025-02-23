using BookManagementAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace BookManagementAPI.Data;

public static class DbInitializer
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var context = new BookDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<BookDbContext>>());

        // Look for any books
        if (await context.Books.AnyAsync())
        {
            return;   // DB has been seeded
        }

        var books = new[]
        {
            new Book
            {
                Title = "The Great Gatsby",
                PublicationYear = 1925,
                AuthorName = "F. Scott Fitzgerald",
                ViewsCount = 150
            },
            new Book
            {
                Title = "To Kill a Mockingbird",
                PublicationYear = 1960,
                AuthorName = "Harper Lee",
                ViewsCount = 200
            },
            new Book
            {
                Title = "1984",
                PublicationYear = 1949,
                AuthorName = "George Orwell",
                ViewsCount = 180
            },
            new Book
            {
                Title = "Pride and Prejudice",
                PublicationYear = 1813,
                AuthorName = "Jane Austen",
                ViewsCount = 120
            },
            new Book
            {
                Title = "The Catcher in the Rye",
                PublicationYear = 1951,
                AuthorName = "J.D. Salinger",
                ViewsCount = 90
            }
        };

        await context.Books.AddRangeAsync(books);
        await context.SaveChangesAsync();
    }
}
