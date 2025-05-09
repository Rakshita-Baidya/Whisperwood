using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Whisperwood.Pages.Review
{
    [Authorize]
    public class AddReviewModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid BookId { get; set; }  
    }
}
