using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace Whisperwood.Controllers
{
    public class BaseController : ControllerBase
    {
        protected Guid GetLoggedInUserId()
        {
            var userIdClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier);
            if (userIdClaim == null || !Guid.TryParse(userIdClaim.Value, out Guid userId))
            {
                throw new UnauthorizedAccessException("User ID not found in token.");
            }
            return userId;
        }
    }
}
