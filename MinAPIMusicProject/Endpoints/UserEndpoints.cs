using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using MinAPIMusicProject.DTOs;
using MinAPIMusicProject.Interfaces;
using MinAPIMusicProject.Models;

namespace MinAPIMusicProject.Endpoints
{
    public static class UserEndpoints
    {
        public static void AddUserEndpoints(this IEndpointRouteBuilder app)
        {
            var endpoint = app.MapGroup("/api/users");

            endpoint.MapPost("/register", async (
                IUserService service,
                [FromBody] UserRegisterRequest userDto,
                CancellationToken cancellationToken = default) =>
            {
                var user = await service.RegisterAsync(userDto.Name, userDto.Login, userDto.Password);
                return Results.Created($"/api/users/{user.Id}", user);
            });

            endpoint.MapPost("/login", async (
                IUserService service,
                [FromBody] UserLoginRequest userDto,
                CancellationToken cancellationToken = default) =>
            {
                var user = await service.LoginAsync(userDto.Login, userDto.Password);
                if (user == null)
                    return Results.Unauthorized();

                return Results.Ok(user);
            });

            endpoint.MapGet("/{userId}/liked-tracks", async (
                IUserService service,
                IMapper mapper,
                [FromRoute] int userId,
                CancellationToken cancellationToken = default) =>
            {
                var likedTracks = await service.GetLikedTracksAsync(userId);
                if (likedTracks == null || likedTracks.Count == 0)
                    return Results.NotFound();

                return Results.Ok(mapper.Map<IEnumerable<TrackDTO>>(likedTracks));
            });

            endpoint.MapPost("/{userId}/like-track/{trackId}", async (
                IUserService service,
                [FromRoute] int userId,
                [FromRoute] int trackId,
                CancellationToken cancellationToken = default) =>
            {
                var success = await service.LikeTrackAsync(userId, trackId);
                if (!success)
                    return Results.NotFound();

                return Results.Ok();
            });
        }
    }
}
