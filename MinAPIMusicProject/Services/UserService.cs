using Microsoft.EntityFrameworkCore;
using MinAPIMusicProject.Data;
using MinAPIMusicProject.Models;

namespace MinAPIMusicProject.Services
{
    public class UserService : IUserService
    {
        private readonly MusicContext _context;

        public UserService(MusicContext context)
        {
            _context = context;
        }

        public async Task<User> RegisterAsync(string name, string login, string password)
        {
            var user = new User { Name = name, Login = login, Password = password };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<User> LoginAsync(string login, string password)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Login == login && u.Password == password);
        }

        public async Task<ICollection<Track>> GetLikedTracksAsync(int userId)
        {
            var user = await _context.Users.Include(u => u.LikedTracks)
                                           .FirstOrDefaultAsync(u => u.Id == userId);
            return user?.LikedTracks.ToList();
        }

        public async Task<bool> LikeTrackAsync(int userId, int trackId)
        {
            var user = await _context.Users.Include(u => u.LikedTracks)
                                           .FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null)
                return false;
            var track = await _context.Tracks.AsNoTracking().FirstOrDefaultAsync(t => t.Id == trackId);
            if (track == null)
                return false;

            if (user.LikedTracks.Any(t => t.Id == trackId))
            {
                user.LikedTracks.Remove(user.LikedTracks.First(t => t.Id == trackId));
            }
            else
            {
                user.LikedTracks.Add(track);
            }

            await _context.SaveChangesAsync();

            return true;
        }

    }
}
