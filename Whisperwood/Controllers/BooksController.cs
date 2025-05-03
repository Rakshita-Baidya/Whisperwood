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
            var user = await dbContext.Users.FindAsync(GetLoggedInUserId());
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can add books.");
            }

            if (await dbContext.Books.AnyAsync(b => b.ISBN == dto.ISBN))
            {
                return BadRequest("A book with this ISBN already exists.");
            }


            // Create book
            var book = new Books
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                ISBN = dto.ISBN,
                Price = dto.Price,
                Synopsis = dto.Synopsis,
                CoverImageId = dto.CoverImageId,
                PublishedDate = dto.PublishedDate,
                Stock = dto.Stock,
                Language = dto.Language,
                Format = dto.Format ?? Books.BookFormat.Paperback,
                Edition = dto.Edition ?? 1
            };

            // Add related entities
            book.AuthorBooks = dto.AuthorIds.Select(id => new AuthorBooks { AuthorId = id, BookId = book.Id }).ToList();
            book.GenreBooks = dto.GenreIds.Select(id => new GenreBooks { GenreId = id, BookId = book.Id }).ToList();
            book.PublisherBooks = dto.PublisherIds.Select(id => new PublisherBooks { PublisherId = id, BookId = book.Id }).ToList();
            book.CategoryBooks = dto.CategoryIds.Select(id => new CategoryBooks { CategoryId = id, BookId = book.Id }).ToList();

            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync();

            return Ok(book);
        }
        [HttpGet("getall")]
        public async Task<IActionResult> GetAllBooks()
        {
            var books = await dbContext.Books
                .Include(b => b.AuthorBooks).ThenInclude(ab => ab.Author)
                .Include(b => b.GenreBooks).ThenInclude(gb => gb.Genre)
                .Include(b => b.PublisherBooks).ThenInclude(pb => pb.Publisher)
                .Include(b => b.CategoryBooks).ThenInclude(cb => cb.Category)
                .Include(b => b.CoverImage)
                .ToListAsync();

            return Ok(books);
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            var book = await dbContext.Books
                .Include(b => b.AuthorBooks).ThenInclude(ab => ab.Author)
                .Include(b => b.GenreBooks).ThenInclude(gb => gb.Genre)
                .Include(b => b.PublisherBooks).ThenInclude(pb => pb.Publisher)
                .Include(b => b.CategoryBooks).ThenInclude(cb => cb.Category)
                .Include(b => b.CoverImage)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book != null ? Ok(book) : NotFound("Book not found!");
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBook(Guid id, BookUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(GetLoggedInUserId());
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can update books.");
            }
            if (await dbContext.Books.AnyAsync(b => b.ISBN == dto.ISBN))
            {
                return BadRequest("A book with this ISBN already exists.");
            }

            var book = await dbContext.Books
                .Include(b => b.AuthorBooks)
                .Include(b => b.GenreBooks)
                .Include(b => b.PublisherBooks)
                .Include(b => b.CategoryBooks)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return NotFound("Book not found.");

            // Update base fields
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

            if (dto.AuthorIds != null)
            {
                book.AuthorBooks.Clear();
                foreach (var idVal in dto.AuthorIds)
                {
                    book.AuthorBooks.Add(new AuthorBooks { AuthorId = idVal, BookId = book.Id });
                }
            }
            if (dto.GenreIds != null)
            {
                book.GenreBooks.Clear();
                foreach (var idVal in dto.GenreIds)
                {
                    book.GenreBooks.Add(new GenreBooks { GenreId = idVal, BookId = book.Id });
                }
            }
            if (dto.PublisherIds != null)
            {
                book.PublisherBooks.Clear();
                foreach (var idVal in dto.PublisherIds)
                {
                    book.PublisherBooks.Add(new PublisherBooks { PublisherId = idVal, BookId = book.Id });
                }
            }
            if (dto.CategoryIds != null)
            {
                book.CategoryBooks.Clear();
                foreach (var idVal in dto.CategoryIds)
                {
                    book.CategoryBooks.Add(new CategoryBooks { CategoryId = idVal, BookId = book.Id });
                }
            }

            await dbContext.SaveChangesAsync();
            return Ok(book);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var user = await dbContext.Users.FindAsync(GetLoggedInUserId());
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return Unauthorized("Only admins can delete books.");
            }

            var book = await dbContext.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound("Book not found.");
            }

            dbContext.Books.Remove(book);
            await dbContext.SaveChangesAsync();
            return Ok("Deleted successfully.");
        }
    }
}
