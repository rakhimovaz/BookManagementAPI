using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BookManagementAPI.Data;
using BookManagementAPI.Models;
using BookManagementAPI.Models.DTOs;
using System.Net;

namespace BookManagementAPI.Controllers;

[ApiController]
[Route("api/[controller]")]
[Produces("application/json")]
public class BooksController : ControllerBase
{
    private readonly BookDbContext _context;

    public BooksController(BookDbContext context)
    {
        _context = context;
    }
    
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(BookDetailResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<ActionResult<BookDetailResponse>> GetBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        
        if (book == null)
            return NotFound($"Book with ID {id} not found.");

        // Increment view count
        book.ViewsCount++;
        await _context.SaveChangesAsync();

        // Create detailed response
        var response = new BookDetailResponse
        {
            Id = book.Id,
            Title = book.Title,
            PublicationYear = book.PublicationYear,
            AuthorName = book.AuthorName,
            ViewsCount = book.ViewsCount,
            PopularityScore = book.GetPopularityScore(),
            YearsSincePublication = DateTime.UtcNow.Year - book.PublicationYear,
            CreatedAt = book.CreatedAt
        };

        return Ok(response);
    }
    
    [HttpGet("popular")]
    [ProducesResponseType(typeof(PaginatedResponse<BookTitleResponse>), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    public async Task<ActionResult<PaginatedResponse<BookTitleResponse>>> GetPopularBooks(
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 10)
    {
        if (pageNumber < 1)
            return BadRequest("Page number must be greater than 0");
        
        if (pageSize < 1 || pageSize > 100)
            return BadRequest("Page size must be between 1 and 100");

        var query = _context.Books
            .Select(b => new BookTitleResponse
            {
                Title = b.Title,
                ViewsCount = b.ViewsCount,
                PopularityScore = b.GetPopularityScore()
            })
            .OrderByDescending(b => b.PopularityScore);

        var totalCount = await query.CountAsync();
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);

        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        var response = new PaginatedResponse<BookTitleResponse>
        {
            Items = items,
            PageNumber = pageNumber,
            PageSize = pageSize,
            TotalPages = totalPages,
            TotalCount = totalCount
        };

        return Ok(response);
    }
    [HttpPost]
    [ProducesResponseType(typeof(Book), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> AddBook(AddBookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var bookExists = await _context.Books.AnyAsync(b => b.Title == request.Title);
        if (bookExists)
            return Conflict("A book with this title already exists.");

        var book = new Book
        {
            Title = request.Title,
            PublicationYear = request.PublicationYear,
            AuthorName = request.AuthorName
        };

        _context.Books.Add(book);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetBook), new { id = book.Id }, book);
    }
    
    [HttpPost("bulk")]
    [ProducesResponseType(typeof(IEnumerable<Book>), (int)HttpStatusCode.Created)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.Conflict)]
    public async Task<IActionResult> AddBooks(AddBooksRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var books = request.Books.Select(b => new Book
        {
            Title = b.Title,
            PublicationYear = b.PublicationYear,
            AuthorName = b.AuthorName
        }).ToList();

        // Check for duplicate titles in the request
        var distinctTitles = books.Select(b => b.Title).Distinct();
        if (distinctTitles.Count() != books.Count)
            return BadRequest("Duplicate book titles in the request.");

        // Check for existing titles in the database
        var existingTitles = await _context.Books
            .Where(b => books.Select(nb => nb.Title).Contains(b.Title))
            .Select(b => b.Title)
            .ToListAsync();

        if (existingTitles.Any())
            return Conflict($"The following books already exist: {string.Join(", ", existingTitles)}");

        _context.Books.AddRange(books);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(AddBooks), books);
    }
    
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(Book), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.BadRequest)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> UpdateBook(int id, [FromBody] UpdateBookRequest request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var book = await _context.Books.FindAsync(id);
        if (book == null)
            return NotFound($"Book with ID {id} not found.");

        book.Title = request.Title;
        book.PublicationYear = request.PublicationYear;
        book.AuthorName = request.AuthorName;

        await _context.SaveChangesAsync();

        return Ok(book);
    }
    [HttpDelete("{id}")]
    [ProducesResponseType((int)HttpStatusCode.NoContent)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> DeleteBook(int id)
    {
        var book = await _context.Books.FindAsync(id);
        
        if (book == null)
            return NotFound($"Book with ID {id} not found.");

        // Soft delete
        book.IsDeleted = true;
        book.DeletedAt = DateTime.UtcNow;
        
        await _context.SaveChangesAsync();
        return NoContent();
    }
    [HttpPost("{id}/restore")]
    [ProducesResponseType(typeof(BookDetailResponse), (int)HttpStatusCode.OK)]
    [ProducesResponseType((int)HttpStatusCode.NotFound)]
    public async Task<IActionResult> RestoreBook(int id)
    {
        var book = await _context.GetBooksIncludingDeleted()
            .FirstOrDefaultAsync(b => b.Id == id);

        if (book == null)
            return NotFound($"Book with ID {id} not found.");

        if (!book.IsDeleted)
            return BadRequest($"Book with ID {id} is not deleted.");

        book.IsDeleted = false;
        book.DeletedAt = null;
        
        await _context.SaveChangesAsync();

        var response = new BookDetailResponse
        {
            Id = book.Id,
            Title = book.Title,
            PublicationYear = book.PublicationYear,
            AuthorName = book.AuthorName,
            ViewsCount = book.ViewsCount,
            PopularityScore = book.GetPopularityScore(),
            YearsSincePublication = DateTime.UtcNow.Year - book.PublicationYear,
            CreatedAt = book.CreatedAt
        };

        return Ok(response);
    }
    
    [HttpGet("deleted")]
    [ProducesResponseType(typeof(List<BookDetailResponse>), (int)HttpStatusCode.OK)]
    public async Task<ActionResult<List<BookDetailResponse>>> GetDeletedBooks()
    {
        var books = await _context.GetBooksIncludingDeleted()
            .Where(b => b.IsDeleted)
            .Select(b => new BookDetailResponse
            {
                Id = b.Id,
                Title = b.Title,
                PublicationYear = b.PublicationYear,
                AuthorName = b.AuthorName,
                ViewsCount = b.ViewsCount,
                PopularityScore = b.GetPopularityScore(),
                YearsSincePublication = DateTime.UtcNow.Year - b.PublicationYear,
                CreatedAt = b.CreatedAt
            })
            .ToListAsync();

        return Ok(books);
    }
}
