namespace BookManagementAPI.Models.DTOs;

public class BookTitleResponse
{
    public string Title { get; set; } = string.Empty;
    public int ViewsCount { get; set; }
    public double PopularityScore { get; set; }
}
