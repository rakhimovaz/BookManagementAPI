namespace BookManagementAPI.Models.DTOs;

public class BookDetailResponse
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public int PublicationYear { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public int ViewsCount { get; set; }
    public double PopularityScore { get; set; }
    public int YearsSincePublication { get; set; }
    public DateTime CreatedAt { get; set; }
}
