using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : BaseController
    {
        private readonly WhisperwoodDbContext dbContext;

        public BookController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddBook(BookDto dto)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var authors = await dbContext.Authors.Where(a => dto.AuthorIds.Contains(a.Id)).ToListAsync();
            var genres = await dbContext.Genres.Where(g => dto.GenreIds.Contains(g.Id)).ToListAsync();
            var categories = await dbContext.Categories.Where(c => dto.CategoryIds.Contains(c.Id)).ToListAsync();
            var publishers = await dbContext.Publishers.Where(p => dto.PublisherIds.Contains(p.Id)).ToListAsync();

            if (authors.Count != dto.AuthorIds.Count || genres.Count != dto.GenreIds.Count ||
                categories.Count != dto.CategoryIds.Count || publishers.Count != dto.PublisherIds.Count)
            {
                return BadRequest("One or more related IDs are invalid.");
            }

            var book = new Books
            {
                Title = dto.Title,
                ISBN = dto.ISBN,
                Price = dto.Price,
                Synopsis = dto.Synopsis,
                CoverImageId = dto.CoverImageId,
                PublishedDate = dto.PublishedDate,
                Stock = dto.Stock,
                Language = dto.Language,
                Format = dto.Format ?? Books.BookFormat.Paperback,
                Edition = dto.Edition,
                AuthorBooks = authors.Select(a => new AuthorBooks { AuthorId = a.Id }).ToList(),
                GenreBooks = genres.Select(g => new GenreBooks { GenreId = g.Id }).ToList(),
                CategoryBooks = categories.Select(c => new CategoryBooks { CategoryId = c.Id }).ToList(),
                PublisherBooks = publishers.Select(p => new PublisherBooks { PublisherId = p.Id }).ToList()
            };

            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync();
            return Ok(book);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllBooks()
        {
            List<Books> bookList = await dbContext.Books.ToListAsync();
            return Ok(bookList);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await dbContext.Books.FirstOrDefaultAsync(b => b.Id == id);
            return book != null ? Ok(book) : NotFound("Book not found!");
        }

        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateBook(Guid id, BookUpdateDto dto)
        {
            var book = await dbContext.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("Book not found!");
            }

            if (dto.Title != null) book.Title = dto.Title;
            if (dto.ISBN != null) book.ISBN = dto.ISBN;
            if (dto.Price.HasValue) book.Price = dto.Price.Value;
            if (dto.Synopsis != null) book.Synopsis = dto.Synopsis;
            if (dto.CoverImageId.HasValue) book.CoverImageId = dto.CoverImageId;
            if (dto.PublishedDate.HasValue) book.PublishedDate = dto.PublishedDate.Value;
            if (dto.Stock.HasValue) book.Stock = dto.Stock.Value;
            if (dto.Language != null) book.Language = dto.Language;
            if (dto.Format.HasValue) book.Format = dto.Format.Value;
            if (dto.Edition.HasValue) book.Edition = dto.Edition.Value;

            // update relationships
            if (dto.AuthorIds != null)
            {
                book.AuthorBooks.Clear();
                foreach (var aid in dto.AuthorIds)
                    book.AuthorBooks.Add(new AuthorBooks { AuthorId = aid });
            }
            if (dto.GenreIds != null)
            {
                book.GenreBooks.Clear();
                foreach (var gid in dto.GenreIds)
                    book.GenreBooks.Add(new GenreBooks { GenreId = gid });
            }
            if (dto.CategoryIds != null)
            {
                book.CategoryBooks.Clear();
                foreach (var cid in dto.CategoryIds)
                    book.CategoryBooks.Add(new CategoryBooks { CategoryId = cid });
            }
            if (dto.PublisherIds != null)
            {
                book.PublisherBooks.Clear();
                foreach (var pid in dto.PublisherIds)
                    book.PublisherBooks.Add(new PublisherBooks { PublisherId = pid });
            }

            await dbContext.SaveChangesAsync();
            return Ok(book);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var userId = GetLoggedInUserId();
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update announcements.");
            }
            var book = await dbContext.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("Book not found!");
            }

            dbContext.Books.Remove(book);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted successfully");
        }
    }
}
