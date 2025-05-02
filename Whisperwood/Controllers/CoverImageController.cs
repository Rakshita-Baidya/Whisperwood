//using Microsoft.AspNetCore.Mvc;
//using Microsoft.EntityFrameworkCore;
//using Whisperwood.DatabaseContext;
//using Whisperwood.Models;

//namespace Whisperwood.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class CoverImageController : ControllerBase
//    {
//        private readonly WhisperwoodDbContext dbContext;

//        public CoverImageController(WhisperwoodDbContext dbContext)
//        {
//            this.dbContext = dbContext;
//        }

//        [HttpPost("add")]
//        public IActionResult AddCoverImage(CoverImages coverImage)
//        {
//            dbContext.CoverImages.Add(coverImage);
//            dbContext.SaveChanges();
//            return Ok(coverImage);

//        }

//        [HttpGet("getall")]
//        public IActionResult GetAllCoverImages()
//        {
//            List<CoverImages> coverImageList = dbContext.CoverImages.ToList();
//            return Ok(coverImageList);
//        }

//        [HttpGet("getbyid/{id}")]
//        public IActionResult GetCoverImageByISBN(Guid id)
//        {
//            CoverImages? coverImage = dbContext.CoverImages.FirstOrDefault(x => x.Id == id);
//            return coverImage != null ? Ok(coverImage) : NotFound("Cover Image not found! Oh noooooo... Check the id again.");
//        }

//        [HttpPut("updateCoverImage/{id}")]
//        public IActionResult UpdateCoverImage(Guid id, CoverImages coverImage)
//        {
//            CoverImages? coverImageToUpdate = dbContext.CoverImages.FirstOrDefault(x => x.Id == id);
//            if (coverImageToUpdate is not null)
//            {
//                coverImageToUpdate.Book = coverImage.Book;
//                coverImageToUpdate.CoverImageURL = coverImage.CoverImageURL;
//                dbContext.SaveChanges();
//                return Ok(coverImageToUpdate);
//            }
//            return NotFound("Id not found! Oh noooooo... Check the id again.");
//        }


//        [HttpDelete("delete/{id}")]
//        public IActionResult DeleteCoverImage(Guid id)
//        {
//            int rowsAffected = dbContext.CoverImages.Where(x => x.Id == id).ExecuteDelete();
//            return Ok(
//                new
//                {
//                    RowsAffected = rowsAffected,
//                    Message = rowsAffected > 0 ? "Deleted successfully" : "Id not found! Oh noooooo... Check the id again."
//                });
//        }

//    }
//}
