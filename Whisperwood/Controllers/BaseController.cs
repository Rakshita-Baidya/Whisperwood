using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Whisperwood.Controllers
{
    public class BaseController : ControllerBase
    {
        protected Guid GetLoggedInUserId()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                           ?? User.FindFirst("sub")?.Value;

            if (Guid.TryParse(userIdClaim, out var userId))
                return userId;

            throw new UnauthorizedAccessException("Invalid or missing user context.");
        }
    }
}
