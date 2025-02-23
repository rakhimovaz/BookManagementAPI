using System.ComponentModel.DataAnnotations;

namespace BookManagementAPI.Models.DTOs;

public class AddBookRequest
{
    [Required]
    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public int PublicationYear { get; set; }

    [Required]
    [MaxLength(200)]
    public string AuthorName { get; set; } = string.Empty;
}

public class AddBooksRequest
{
    [Required]
    public List<AddBookRequest> Books { get; set; } = new();
}
