using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MinAPIMusicProject.Data;
using MinAPIMusicProject.DTOs;

namespace MinAPIMusicProject.Endpoints;

public static class TrackEndpoints
{
    public static void AddTrackEndpoints(this IEndpointRouteBuilder app)
    {
        var endpoint = app.MapGroup("/api/tracks");

        endpoint.MapGet("/", async (
                MusicContext context,
                IMapper mapper, 
                [FromQuery] int page = 0,
                [FromQuery] int size = 10,
                [FromQuery] string? q = null) =>
        {
            var tracks = q == null ? context.Tracks : context.Tracks.Where(x => x.Title.Contains(q));
            var result = await tracks.Skip(page * size)
                .Take(size)
                .ToListAsync();

            var trackDTOs = mapper.Map<IEnumerable<TrackDTO>>(result);
            return Results.Ok(trackDTOs);
        })
            .WithName("Get tracks endpoint")
            .WithDescription("Get tracks from database...");

        endpoint.MapGet("{id}", async (MusicContext context, IMapper mapper, [FromRoute] int id) =>
        {
            var track = await context.Tracks.FindAsync(id);
            if (track == null)
            {
                return Results.NotFound();
            }

            var trackDTO = mapper.Map<TrackDTO>(track);
            return Results.Ok(trackDTO);
        });

        endpoint.MapPost("{id}/play", async (
            MusicContext context,
            IMapper mapper,
            [FromRoute] int id,
            CancellationToken cancellationToken = default) =>
        {
            var track = await context.Tracks.FindAsync(id);

            if (track == null)
            {
                return Results.NotFound();
            }

            track.Listened++;
            await context.SaveChangesAsync(cancellationToken);

            var trackDTO = mapper.Map<TrackDTO>(track);
            return Results.Ok(trackDTO);
        });

        endpoint.MapGet("/recommendations", async (
            MusicContext context,
            IMapper mapper,
            [FromQuery] int limit = 10,
            [FromQuery] string genres = "",
            [FromQuery] int minDuration = 1,
            [FromQuery] int maxDuration = int.MaxValue,
            CancellationToken cancellationToken = default) =>
        {
            var genreList = genres.Split(',', StringSplitOptions.RemoveEmptyEntries);

            var tracks = await context.Tracks.Where(x =>
                x.DurationInSeconds >= minDuration && x.DurationInSeconds <= maxDuration &&
                (!genreList.Any() || genreList.Contains(x.Genre.Name.ToLower())))
                .OrderByDescending(x => x.Listened)
                .Take(limit)
                .ToListAsync(cancellationToken);

            var trackDTOs = mapper.Map<IEnumerable<TrackDTO>>(tracks);
            return Results.Ok(trackDTOs);
        });
    }
}
