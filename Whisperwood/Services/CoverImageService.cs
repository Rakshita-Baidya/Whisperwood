﻿using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Whisperwood.DatabaseContext;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Services
{
    public class CoverImageService : ICoverImageService
    {
        private readonly WhisperwoodDbContext dbContext;

        public CoverImageService(WhisperwoodDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public async Task<IActionResult> AddCoverImageAsync(Guid userId, CoverImageDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can add cover images."
                    });
                }
            }

            var coverImage = new CoverImages
            {
                Id = Guid.NewGuid(),
                CoverImageURL = dto.CoverImageURL
            };

            dbContext.CoverImages.Add(coverImage);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(coverImage);
        }

        public async Task<IActionResult> GetAllCoverImagesAsync()
        {
            var coverImageList = await dbContext.CoverImages.ToListAsync();
            return new OkObjectResult(coverImageList);
        }

        public async Task<IActionResult> GetCoverImageByIdAsync(Guid id)
        {
            var coverImage = await dbContext.CoverImages.FirstOrDefaultAsync(c => c.Id == id);
            return coverImage != null ? new OkObjectResult(coverImage) : new NotFoundObjectResult(new
            {
                message = "Cover Image not found! Please check the id again."
            });
        }

        public async Task<IActionResult> UpdateCoverImageAsync(Guid userId, Guid id, CoverImageUpdateDto dto)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can update cover images."
                    });
                }
            }

            var coverImage = await dbContext.CoverImages.FindAsync(id);
            if (coverImage == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Cover Image not found!"
                });
            }

            if (dto.CoverImageURL != null) coverImage.CoverImageURL = dto.CoverImageURL;

            await dbContext.SaveChangesAsync();
            return new OkObjectResult(coverImage);
        }

        public async Task<IActionResult> DeleteCoverImageAsync(Guid userId, Guid id)
        {
            var user = await dbContext.Users.FindAsync(userId);
            if (user != null)
            {
                if (!user.IsAdmin.GetValueOrDefault(false) && !user.IsStaff.GetValueOrDefault(false))
                {
                    return new UnauthorizedObjectResult(new
                    {
                        message = "Only admins or staff can delete cover images."
                    });
                }
            }

            var coverImage = await dbContext.CoverImages.FindAsync(id);
            if (coverImage == null)
            {
                return new NotFoundObjectResult(new
                {
                    message = "Cover Image not found!"
                });
            }

            dbContext.CoverImages.Remove(coverImage);
            await dbContext.SaveChangesAsync();
            return new OkObjectResult(new
            {
                message = "Deleted successfully!"
            });
        }
    }
}
