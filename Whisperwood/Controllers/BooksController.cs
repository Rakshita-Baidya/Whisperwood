using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BookController : BaseController
    {
        private readonly IBookService bookService;

        public BookController(IBookService bookService)
        {
            this.bookService = bookService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddBook(BookDto dto)
        {
            var userId = GetLoggedInUserId();
            return await bookService.AddBook(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllBooks()
        {
            return await bookService.GetAllBooks();
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetBookById(Guid id)
        {
            return await bookService.GetBookById(id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateBook(Guid id, BookUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await bookService.UpdateBook(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBook(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await bookService.DeleteBook(userId, id);
        }
    }
}
