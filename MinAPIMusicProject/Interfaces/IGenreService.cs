using MinAPIMusicProject.Models;

namespace MinAPIMusicProject.Interfaces
{
    public interface IGenreService
    {
        Task<Genre> AddGenre(Genre genre, CancellationToken cancellationToken = default);
        Task DeleteGenre(int id, CancellationToken cancellationToken = default);
        Task<List<Genre>> GetGenres(int page, int size, string? q, CancellationToken cancellationToken = default);

    }
}
