﻿using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public class InMemoryGamesRepository : IGamesRepository
{
    private readonly List<Game> games = new()
        {
            new Game()
            {
                Id = 1,
                Name = "Street Fighter II",
                Genre = "Fighting",
                Price = 19.99M,
                ReleaseDate = new DateTime(1991, 2, 1),
                ImageUri = "https://placehold.co/100"
            },
            new Game()
            {
                Id = 2,
                Name = "Final Fantasy XIV",
                Genre = "Roleplaying",
                Price = 59.99M,
                ReleaseDate = new DateTime(2010, 9, 30),
                ImageUri = "https://placehold.co/100"
            },
            new Game(){
                Id = 3,
                Name = "FIFA 23",
                Genre = "Sports",
                Price = 69.99M,
                ReleaseDate = new DateTime(2022, 9, 27),
                ImageUri = "https://placehold.co/100"
            }
        };

    public async Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize, string? filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;
        return await Task.FromResult(FilterGames(filter).Skip(skipCount).Take(pageSize));
    }

    public async Task<Game?> GetAsync(int id)
    {
        return await Task.FromResult(games.Find(g => g.Id == id));
    }

    public async Task CreateAsync(Game game)
    {
        game.Id = games.Count() == 0 ? 1 : games.Max(g => g.Id) + 1;
        games.Add(game);

        await Task.CompletedTask;
    }

    public async Task UpdateAsync(Game updatedGame)
    {
        var index = games.FindIndex(g => g.Id == updatedGame.Id);
        games[index] = updatedGame;

        await Task.CompletedTask;
    }

    public async Task DeleteAsync(int id)
    {
        var index = games.FindIndex(g => g.Id == id);
        games.RemoveAt(index);

        await Task.CompletedTask;
    }

    public async Task<int> CountAsync(string? filter)
    {
        return await Task.FromResult(FilterGames(filter).Count());
    }

    private IEnumerable<Game> FilterGames(string? filter)
    {
        if (filter is null)
        {
            return games;
        }

        return games.Where(game => game.Name.Contains(filter) || game.Genre.Contains(filter));
    }
}


