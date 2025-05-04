using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class BookService : IBookService
    {
        private readonly WhisperwoodDbContext dbContext;

        public BookService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddBookAsync(Guid userId, BookDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can add books.");
            }

            if (await dbContext.Books.AnyAsync(b => b.ISBN == dto.ISBN))
            {
                return new BadRequestObjectResult("A book with this ISBN already exists.");
            }

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

            book.AuthorBooks = dto.AuthorIds.Select(id => new AuthorBooks { AuthorId = id, BookId = book.Id }).ToList();
            book.GenreBooks = dto.GenreIds.Select(id => new GenreBooks { GenreId = id, BookId = book.Id }).ToList();
            book.PublisherBooks = dto.PublisherIds.Select(id => new PublisherBooks { PublisherId = id, BookId = book.Id }).ToList();
            book.CategoryBooks = dto.CategoryIds.Select(id => new CategoryBooks { CategoryId = id, BookId = book.Id }).ToList();

            dbContext.Books.Add(book);
            await dbContext.SaveChangesAsync();

            return new OkObjectResult(book);
        }

        public async Task<IActionResult> GetAllBooksAsync()
        {
            var books = await dbContext.Books
                .Include(b => b.AuthorBooks).ThenInclude(ab => ab.Author)
                .Include(b => b.GenreBooks).ThenInclude(gb => gb.Genre)
                .Include(b => b.PublisherBooks).ThenInclude(pb => pb.Publisher)
                .Include(b => b.CategoryBooks).ThenInclude(cb => cb.Category)
                .Include(b => b.CoverImage)
                .ToListAsync();

            return new OkObjectResult(books);
        }

        public async Task<IActionResult> GetBookByIdAsync(Guid id)
        {
            var book = await dbContext.Books
                .Include(b => b.AuthorBooks).ThenInclude(ab => ab.Author)
                .Include(b => b.GenreBooks).ThenInclude(gb => gb.Genre)
                .Include(b => b.PublisherBooks).ThenInclude(pb => pb.Publisher)
                .Include(b => b.CategoryBooks).ThenInclude(cb => cb.Category)
                .Include(b => b.CoverImage)
                .FirstOrDefaultAsync(b => b.Id == id);

            return book != null ? new OkObjectResult(book) : new NotFoundObjectResult("Book not found!");
        }

        public async Task<IActionResult> UpdateBookAsync(Guid userId, Guid id, BookUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can update books.");
            }

            if (dto.ISBN != null && await dbContext.Books.AnyAsync(b => b.ISBN == dto.ISBN && b.Id != id))
            {
                return new BadRequestObjectResult("A book with this ISBN already exists.");
            }

            var book = await dbContext.Books
                .Include(b => b.AuthorBooks)
                .Include(b => b.GenreBooks)
                .Include(b => b.PublisherBooks)
                .Include(b => b.CategoryBooks)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return new NotFoundObjectResult("Book not found.");

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
                book.AuthorBooks = dto.AuthorIds.Select(idVal => new AuthorBooks { AuthorId = idVal, BookId = book.Id }).ToList();
            }
            if (dto.GenreIds != null)
            {
                book.GenreBooks.Clear();
                book.GenreBooks = dto.GenreIds.Select(idVal => new GenreBooks { GenreId = idVal, BookId = book.Id }).ToList();
            }
            if (dto.PublisherIds != null)
            {
                book.PublisherBooks.Clear();
                book.PublisherBooks = dto.PublisherIds.Select(idVal => new PublisherBooks { PublisherId = idVal, BookId = book.Id }).ToList();
            }
            if (dto.CategoryIds != null)
            {
                book.CategoryBooks.Clear();
                book.CategoryBooks = dto.CategoryIds.Select(idVal => new CategoryBooks { CategoryId = idVal, BookId = book.Id }).ToList();
            }

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(book);
        }

        public async Task<IActionResult> DeleteBookAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user == null || !user.IsAdmin.GetValueOrDefault(false))
            {
                return new UnauthorizedObjectResult("Only admins can delete books.");
            }

            var book = await dbContext.Books.FindAsync(id);
            if (book == null)
            {
                return new NotFoundObjectResult("Book not found.");
            }

            dbContext.Books.Remove(book);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult("Deleted successfully.");
        }
    }
}
