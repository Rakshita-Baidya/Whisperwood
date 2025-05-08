using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Whisperwood.Pages.Review
{
    [Authorize]
    public class EditReviewModel : PageModel
    {
        [BindProperty(SupportsGet = true)]
        public Guid Id { get; set; }  
    }
}
