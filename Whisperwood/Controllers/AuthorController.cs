using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthorController : BaseController
    {
        private readonly IAuthorService authorService;

        public AuthorController(IAuthorService authorService)
        {
            this.authorService = authorService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddAuthor(AuthorDto dto)
        {
            var userId = GetLoggedInUserId();
            return await authorService.AddAuthorAsync(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllAuthors()
        {
            return await authorService.GetAllAuthorsAsync();
        }

        [HttpGet("getbyid/{id}")]
        [Authorize]
        public async Task<IActionResult> GetAuthorById(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await authorService.GetAuthorByIdAsync(userId, id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateAuthor(Guid id, AuthorUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await authorService.UpdateAuthorAsync(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteAuthor(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await authorService.DeleteAuthorAsync(userId, id);
        }
    }
}