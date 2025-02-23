namespace BookManagementAPI.Models.DTOs
{
    public class UpdateBookRequest
    {
        public string Title { get; set; }
        public int PublicationYear { get; set; }
        public string AuthorName { get; set; }
    }
}