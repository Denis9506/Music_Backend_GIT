using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinAPIMusicProject.Data;
using MinAPIMusicProject.Interfaces;
using MinAPIMusicProject.Models;

namespace MinAPIMusicProject.Services
{
    public class GenreService : IGenreService
    {
        private readonly MusicContext _context;
        private readonly IMapper _mapper;

        public GenreService(MusicContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<Genre> AddGenre(Genre genre, CancellationToken cancellationToken = default)
        {
            var genreFromDb = await _context.Genres.AddAsync(genre, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);

            return genreFromDb.Entity;
        }

        public async Task DeleteGenre(int id, CancellationToken cancellationToken = default)
        {
            var genre = await _context.Genres.FindAsync(new object[] { id }, cancellationToken);

            if (genre == null)
            {
                throw new ArgumentNullException(nameof(genre), $"Genre with ID {id} not found.");
            }

            _context.Genres.Remove(genre);
            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<Genre>> GetGenres(int page, int size, string? q, CancellationToken cancellationToken = default)
        {
            var genresDb = string.IsNullOrWhiteSpace(q)
                 ? _context.Genres
                 : _context.Genres.Where(g => g.Name.Contains(q));

            var genres = await genresDb
                .Skip(page * size)
                .Take(size)
                .Select(g => new Genre
                {
                    Id = g.Id,
                    Name = g.Name,
                })
                .ToListAsync(cancellationToken);

            return genres;
        }
    }
}
