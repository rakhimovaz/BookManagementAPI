using System.ComponentModel.DataAnnotations;

namespace BookManagementAPI.Models;

public class Book
{
    public int Id { get; set; }

    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int PublicationYear { get; set; }

    [Required]
    [MaxLength(200)]
    public string AuthorName { get; set; } = string.Empty;

    public int ViewsCount { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsDeleted { get; set; } = false;

    public DateTime? DeletedAt { get; set; }

    public double GetPopularityScore()
    {
        var yearsSincePublished = DateTime.UtcNow.Year - PublicationYear;
        return (ViewsCount * 0.5) + (yearsSincePublished * 2);
    }
}
