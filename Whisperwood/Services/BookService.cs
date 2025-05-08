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
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new { message = "Only admins or staff can add books." });
                }
            }

            if (await dbContext.Books.AnyAsync(b => b.ISBN == dto.ISBN))
            {
                return new BadRequestObjectResult(new { message = "A book with this ISBN already exists." });
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

            return book != null ? new OkObjectResult(book) : new NotFoundObjectResult(new { message = "Book not found!" });
        }

        public async Task<IActionResult> UpdateBookAsync(Guid userId, Guid id, BookUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new { message = "Only admins or staff can update books." });
                }
            }

            if (dto.ISBN != null && await dbContext.Books.AnyAsync(b => b.ISBN == dto.ISBN && b.Id != id))
            {
                return new BadRequestObjectResult(new { message = "A book with this ISBN already exists." });
            }

            var book = await dbContext.Books
                .Include(b => b.AuthorBooks)
                .Include(b => b.GenreBooks)
                .Include(b => b.PublisherBooks)
                .Include(b => b.CategoryBooks)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book == null)
                return new NotFoundObjectResult(new { message = "Book not found." });

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
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new { message = "Only admins or staff can delete books." });
                }
            }

            var book = await dbContext.Books.FindAsync(id);
            if (book == null)
            {
                return new NotFoundObjectResult(new { message = "Book not found." });
            }

            dbContext.Books.Remove(book);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new { message = "Deleted successfully." });
        }

        public async Task<IActionResult> GetFilteredBooksAsync(BookFilterDto filter)
        {
            var query = dbContext.Books
                .Include(b => b.AuthorBooks).ThenInclude(ab => ab.Author)
                .Include(b => b.GenreBooks).ThenInclude(gb => gb.Genre)
                .Include(b => b.PublisherBooks).ThenInclude(pb => pb.Publisher)
                .AsNoTracking();

            // Apply Filters
            if (filter.AuthorIds != null && filter.AuthorIds.Any())
            {
                query = query.Where(b => b.AuthorBooks.Any(ab => filter.AuthorIds.Contains(ab.AuthorId)));
            }

            if (filter.GenreIds != null && filter.GenreIds.Any())
            {
                query = query.Where(b => b.GenreBooks.Any(gb => filter.GenreIds.Contains(gb.GenreId)));
            }

            if (filter.IsAvailable.HasValue && filter.IsAvailable.Value)
            {
                query = query.Where(b => b.Stock > 0 && b.AvailabilityStatus);
            }

            if (filter.MinPrice.HasValue)
            {
                query = query.Where(b => b.Price >= filter.MinPrice.Value);
            }

            if (filter.MaxPrice.HasValue)
            {
                query = query.Where(b => b.Price <= filter.MaxPrice.Value);
            }

            if (filter.MinRating.HasValue)
            {
                query = query.Where(b => b.AverageRating >= filter.MinRating.Value);
            }

            if (!string.IsNullOrEmpty(filter.Language))
            {
                query = query.Where(b => b.Language != null && b.Language.Equals(filter.Language, StringComparison.OrdinalIgnoreCase));
            }

            if (filter.Formats != null && filter.Formats.Any())
            {
                query = query.Where(b => filter.Formats.Contains(b.Format));
            }

            if (filter.PublisherIds != null && filter.PublisherIds.Any())
            {
                query = query.Where(b => b.PublisherBooks.Any(pb => filter.PublisherIds.Contains(pb.PublisherId)));
            }

            // Apply Search
            if (!string.IsNullOrEmpty(filter.SearchTerm))
            {
                var searchTerm = filter.SearchTerm.ToLower();
                query = query.Where(b =>
                    b.Title.ToLower().Contains(searchTerm) ||
                    b.ISBN.Contains(searchTerm) ||
                    (b.Synopsis != null && b.Synopsis.ToLower().Contains(searchTerm)));
            }

            // Apply Sorting
            if (filter.SortBy.HasValue)
            {
                switch (filter.SortBy.Value)
                {
                    case BookFilterDto.SortByOption.Title:
                        query = filter.SortOrder == BookFilterDto.SortOrders.Descending
                            ? query.OrderByDescending(b => b.Title)
                            : query.OrderBy(b => b.Title);
                        break;
                    case BookFilterDto.SortByOption.PublicationDate:
                        query = filter.SortOrder == BookFilterDto.SortOrders.Descending
                            ? query.OrderByDescending(b => b.PublishedDate)
                            : query.OrderBy(b => b.PublishedDate);
                        break;
                    case BookFilterDto.SortByOption.Price:
                        query = filter.SortOrder == BookFilterDto.SortOrders.Descending
                            ? query.OrderByDescending(b => b.Price)
                            : query.OrderBy(b => b.Price);
                        break;
                    case BookFilterDto.SortByOption.Popularity:
                        query = filter.SortOrder == BookFilterDto.SortOrders.Descending
                            ? query.OrderByDescending(b => b.SalesCount)
                            : query.OrderBy(b => b.SalesCount);
                        break;
                }
            }

            var books = await query.ToListAsync();
            return new OkObjectResult(books);
        }
    }
}
