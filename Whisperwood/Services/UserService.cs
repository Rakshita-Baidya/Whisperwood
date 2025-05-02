using Whisperwood.DatabaseContext;

namespace Whisperwood.Services
{
    public class UserService
    {
        private readonly WhisperwoodDbContext _context;

        public UserService(WhisperwoodDbContext context)
        {
            _context = context;
        }

    }
}
