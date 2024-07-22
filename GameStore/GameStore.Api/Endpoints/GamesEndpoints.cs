using GameStore.Api.Authorization;
using GameStore.Api.Dtos;
using GameStore.Api.Entities;
using GameStore.Api.Repositories;
using Microsoft.AspNetCore.Http.HttpResults;

namespace GameStore.Api.Endpoints;

public static class GamesEndpoints
{
    const string GetGameV1EndpointName = "GetGameV1";
    const string GetGameV2EndpointName = "GetGameV2";

    public static RouteGroupBuilder MapGamesEndpoints(this IEndpointRouteBuilder routes)
    {
        var group = routes.NewVersionedApi()
                        .MapGroup("/games")
                        .HasApiVersion(1, 0)
                        .HasApiVersion(2, 0)
                        .WithParameterValidation();

        // V1 GET ENDPOINTS
        group.MapGet("/", async (
            IGamesRepository repository,
            ILoggerFactory loggerFactory,
            [AsParameters] GetGamesDtoV1 request,
            HttpContext http) =>
        {
            var totalCount = await repository.CountAsync(request.Filter);
            http.Response.AddPaginationHeader(totalCount, request.PageSize);

            return Results.Ok((await repository.GetAllAsync(
                request.PageNumber,
                request.PageSize,
                request.Filter))
                .Select(game => game.AsDtoV1()));
        })
        .MapToApiVersion(1.0);

        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV1>, NotFound>> (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);
            return game is not null ? TypedResults.Ok(game.AsDtoV1()) : TypedResults.NotFound();
        })
        .WithName(GetGameV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0);

        // V2 GET ENDPOINTS
        group.MapGet("/", async (
            IGamesRepository repository,
            ILoggerFactory loggerFactory,
            [AsParameters] GetGamesDtoV2 request,
            HttpContext http) =>
        {
            var totalCount = await repository.CountAsync(request.Filter);
            http.Response.AddPaginationHeader(totalCount, request.PageSize);

            return Results.Ok((await repository.GetAllAsync(
                request.PageNumber,
                request.PageSize,
                request.Filter))
                .Select(game => game.AsDtoV2()));
        })
        .MapToApiVersion(2.0);

        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV2>, NotFound>> (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);
            return game is not null ? TypedResults.Ok(game.AsDtoV2()) : TypedResults.NotFound();
        })
        .WithName(GetGameV2EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(2.0);

        group.MapPost("/", async Task<CreatedAtRoute<GameDtoV1>> (IGamesRepository repository, CreateGameDto CreatedGameDto) =>
        {
            Game game = new()
            {
                Name = CreatedGameDto.Name,
                Genre = CreatedGameDto.Genre,
                Price = CreatedGameDto.Price,
                ReleaseDate = CreatedGameDto.ReleaseDate,
                ImageUri = CreatedGameDto.ImageUri
            };

            await repository.CreateAsync(game);
            return TypedResults.CreatedAtRoute(game.AsDtoV1(), GetGameV1EndpointName, new { id = game.Id });
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0);

        group.MapPut("/{id}", async Task<Results<NotFound, NoContent>> (IGamesRepository repository, int id, UpdateGameDto updateGameDto) =>
        {
            Game? existingGame = await repository.GetAsync(id);

            if (existingGame is null)
            {
                return TypedResults.NotFound();
            }

            existingGame.Name = updateGameDto.Name;
            existingGame.Genre = updateGameDto.Genre;
            existingGame.Price = updateGameDto.Price;
            existingGame.ReleaseDate = updateGameDto.ReleaseDate;
            existingGame.ImageUri = updateGameDto.ImageUri;

            await repository.UpdateAsync(existingGame);

            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0);

        group.MapDelete("/{id}", async (IGamesRepository repository, int id) =>
        {
            Game? existingGame = await repository.GetAsync(id);

            if (existingGame is not null)
            {
                await repository.DeleteAsync(id);
            }

            return TypedResults.NoContent();
        })
        .RequireAuthorization(Policies.WriteAccess)
        .MapToApiVersion(1.0);

        return group;
    }
}
