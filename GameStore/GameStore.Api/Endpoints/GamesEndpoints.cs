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
                        .WithParameterValidation()
                        .WithOpenApi()
                        .WithTags("Games");

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
        .MapToApiVersion(1.0)
        .WithSummary("Gets all games")
        .WithDescription("Gets all available games and allows filtering and pagination");

        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV1>, NotFound>> (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);
            return game is not null ? TypedResults.Ok(game.AsDtoV1()) : TypedResults.NotFound();
        })
        .WithName(GetGameV1EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(1.0)
        .WithSummary("Gets a game by id")
        .WithDescription("Gets the game that has the specified id");

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
        .MapToApiVersion(2.0)
        .WithSummary("Gets all games")
        .WithDescription("Gets all available games and allows filtering and pagination"); ;

        group.MapGet("/{id}", async Task<Results<Ok<GameDtoV2>, NotFound>> (IGamesRepository repository, int id) =>
        {
            Game? game = await repository.GetAsync(id);
            return game is not null ? TypedResults.Ok(game.AsDtoV2()) : TypedResults.NotFound();
        })
        .WithName(GetGameV2EndpointName)
        .RequireAuthorization(Policies.ReadAccess)
        .MapToApiVersion(2.0)
        .WithSummary("Gets a game by id")
        .WithDescription("Gets the game that has the specified id"); ;

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
        .MapToApiVersion(1.0)
        .WithSummary("Creates a new game")
        .WithDescription("Creates a new game with the specified details");

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
        .MapToApiVersion(1.0)
        .WithSummary("Updates a game")
        .WithDescription("Updates all game properties for the game that has the specified id");

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
        .MapToApiVersion(1.0)
        .WithSummary("Deletes a game")
        .WithDescription("Deletes the game that has the specified id");

        return group;
    }
}
