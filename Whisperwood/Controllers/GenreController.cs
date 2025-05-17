using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenreController : BaseController
    {
        private readonly IGenreService genreService;

        public GenreController(IGenreService genreService)
        {
            this.genreService = genreService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddGenre(GenreDto dto)
        {
            var userId = GetLoggedInUserId();
            return await genreService.AddGenreAsync(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllGenres()
        {
            return await genreService.GetAllGenresAsync();
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetGenreById(Guid id)
        {
            return await genreService.GetGenreByIdAsync(id);
        }

        [HttpPut("update/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateGenre(Guid id, GenreUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await genreService.UpdateGenreAsync(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteGenre(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await genreService.DeleteGenreAsync(userId, id);
        }
    }
}
