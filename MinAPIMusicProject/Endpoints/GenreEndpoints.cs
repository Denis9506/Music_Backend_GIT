using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MinAPIMusicProject.Data;
using MinAPIMusicProject.DTOs;
using MinAPIMusicProject.Interfaces;
using MinAPIMusicProject.Models;

namespace MinAPIMusicProject.Endpoints
{
    public static class GenreEndpoints
    {
        public static void AddGenreEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoint = app.MapGroup("/api/genres");

            endpoint.MapGet("/", async (
                IGenreService service,
                 [FromQuery] int page = 0,
                 [FromQuery] int size = 10,
                 [FromQuery] string? q = null,
                CancellationToken cancellationToken = default) =>
            {
                var result = await service.GetGenres(page, size, q, cancellationToken);

                return Results.Ok(result);
            });

           
            endpoint.MapPost("/", async (
                IGenreService service,
                Genre genreDto,
                CancellationToken cancellationToken = default) =>
            {
                var genre = new Genre { Name = genreDto.Name };
                var genreFromDb = await service.AddGenre(genre, cancellationToken);

                return Results.Created($"/api/genres/{genreFromDb.Id}", genreFromDb.Id);
            });

            endpoint.MapDelete("{id}", async (
                IGenreService service,
                [FromRoute] int id,
                CancellationToken cancellationToken = default) =>
            {
                try
                {
                    await service.DeleteGenre(id, cancellationToken);
                    return Results.Ok();
                }
                catch (ArgumentNullException)
                {
                    return Results.NotFound();
                }
                catch (Exception ex)
                {
                    return Results.BadRequest(ex.Message);
                }
            });
        }
    }
}
