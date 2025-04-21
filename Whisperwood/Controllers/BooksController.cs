using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.Models;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BooksController : ControllerBase
    {
        private readonly WhisperwoodDbContext dbContext;

        public BooksController(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        [HttpPost("add")]
        public IActionResult AddBook(Books book)
        {
            dbContext.Books.Add(book);
            dbContext.SaveChanges();
            return Ok(book);

        }

        [HttpGet("getall")]
        public IActionResult GetAllBooks()
        {
            List<Books> bookList = dbContext.Books.ToList();
            return Ok(bookList);
        }

        [HttpGet("getbyid/{id}")]
        public IActionResult GetBookById(Guid id)
        {
            Books? book = dbContext.Books.FirstOrDefault(x => x.Id == id);
            return book != null ? Ok(book) : NotFound("Book not found! Oh noooooo... Check the id again.");
        }

        [HttpPut("updateBook/{id}")]
        public IActionResult UpdateBook(Guid id, Books book)
        {
            Books? bookToUpdate = dbContext.Books.FirstOrDefault(x => x.Id == id);
            if (bookToUpdate is not null)
            {
                bookToUpdate.Title = book.Title;
                bookToUpdate.ISBN = book.ISBN;
                bookToUpdate.Price = book.Price;
                bookToUpdate.Synopsis = book.Synopsis;
                bookToUpdate.Stock = book.Stock;
                bookToUpdate.AvailablilityStatus = book.AvailablilityStatus;
                bookToUpdate.Language = book.Language;
                bookToUpdate.Format = book.Format;
                bookToUpdate.CoverImage = book.CoverImage;
                bookToUpdate.Edition = book.Edition;
                bookToUpdate.AverageRating = book.AverageRating;
                bookToUpdate.PublishedDate = book.PublishedDate;
                dbContext.SaveChanges();
                return Ok(bookToUpdate);
            }
            return NotFound("Id not found! Oh noooooo... Check the id again.");
        }


        [HttpDelete("delete/{id}")]
        public IActionResult DeleteBook(Guid id)
        {
            int rowsAffected = dbContext.Books.Where(x => x.Id == id).ExecuteDelete();
            return Ok(
                new
                {
                    RowsAffected = rowsAffected,
                    Message = rowsAffected > 0 ? "Deleted successfully" : "Id not found! Oh noooooo... Check the id again."
                });
        }
    }
}
