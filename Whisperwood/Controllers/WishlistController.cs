using Microsoft.AspNetCore.Mvc;

namespace Whisperwood.Controllers
{
    public class WishlistController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
