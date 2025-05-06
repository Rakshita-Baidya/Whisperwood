using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using Whisperwood.DTOs;
using Whisperwood.Interfaces;
using Whisperwood.Models;

namespace Whisperwood.Pages.Review
{
    [Authorize]
    public class BookReviewModel : PageModel
    {
        private readonly IReviewService _reviewService;

        public BookReviewModel(IReviewService reviewService)
        {
            _reviewService = reviewService;
        }

        public List<Reviews> Reviews { get; set; } = new();

        [BindProperty]
        public ReviewDTO NewReview { get; set; }

        [BindProperty(SupportsGet = true)]
        public Guid BookId { get; set; }

        public Guid CurrentUserId =>
            Guid.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var id) ? id : Guid.Empty;

        public async Task<IActionResult> OnGetAsync()
        {
            var result = await _reviewService.GetReviewsByBookAsync(BookId);
            if (result is OkObjectResult ok && ok.Value is List<Reviews> reviews)
            {
                Reviews = reviews;
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            NewReview.BookId = BookId;
            var result = await _reviewService.AddReviewAsync(CurrentUserId, NewReview);

            if (result is OkObjectResult)
                return RedirectToPage(new { BookId });

            return Page();
        }

        public async Task<IActionResult> OnPostDeleteAsync(Guid id)
        {
            await _reviewService.DeleteReviewAsync(CurrentUserId, id);
            return RedirectToPage(new { BookId });
        }
    }
}
