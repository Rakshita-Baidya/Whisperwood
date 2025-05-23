﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;

namespace Whisperwood.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CoverImageController : BaseController
    {
        private readonly ICoverImageService coverImageService;

        public CoverImageController(ICoverImageService coverImageService)
        {
            this.coverImageService = coverImageService;
        }

        [HttpPost("add")]
        [Authorize]
        public async Task<IActionResult> AddCoverImage(CoverImageDto dto)
        {
            var userId = GetLoggedInUserId();
            return await coverImageService.AddCoverImageAsync(userId, dto);
        }

        [HttpGet("getall")]
        public async Task<IActionResult> GetAllCoverImages()
        {
            return await coverImageService.GetAllCoverImagesAsync();
        }

        [HttpGet("getbyid/{id}")]
        public async Task<IActionResult> GetCoverImageById(Guid id)
        {
            return await coverImageService.GetCoverImageByIdAsync(id);
        }

        [HttpPut("updateCoverImage/{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateCoverImage(Guid id, CoverImageUpdateDto dto)
        {
            var userId = GetLoggedInUserId();
            return await coverImageService.UpdateCoverImageAsync(userId, id, dto);
        }

        [HttpDelete("delete/{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteCoverImage(Guid id)
        {
            var userId = GetLoggedInUserId();
            return await coverImageService.DeleteCoverImageAsync(userId, id);
        }
    }
}
