using MinAPIMusicProject.Models;

public interface IUserService
{
    Task<User> RegisterAsync(string name, string login, string password);
    Task<User> LoginAsync(string login, string password);
    Task<ICollection<Track>> GetLikedTracksAsync(int userId);
    Task<bool> LikeTrackAsync(int userId, int trackId);
}