using GameStore.Api.Data;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Repositories;

public class EntityFrameworkGamesRepository : IGamesRepository
{
    private readonly GameStoreContext _dbContext;
    private readonly ILogger<EntityFrameworkGamesRepository> _logger;

    public EntityFrameworkGamesRepository(
        GameStoreContext dbContext,
        ILogger<EntityFrameworkGamesRepository> logger)
    {
        _dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize, string? filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await FilterGames(filter)
                        .OrderBy(game => game.Id)
                        .Skip(skipCount)
                        .Take(pageSize)
                        .AsNoTracking().ToListAsync();
    }

    public async Task<Game?> GetAsync(int id)
    {
        return await _dbContext.Games.FindAsync(id);
    }

    public async Task CreateAsync(Game game)
    {
        _dbContext.Games.Add(game);
        await _dbContext.SaveChangesAsync();

        _logger.LogInformation("Created game {Name} with price {Price}.", game.Name, game.Price);
    }

    public async Task UpdateAsync(Game updatedGame)
    {
        _dbContext.Games.Update(updatedGame);
        await _dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await _dbContext.Games.Where(game => game.Id == id)
                        .ExecuteDeleteAsync();
    }

    public async Task<int> CountAsync(string? filter)
    {
        return await FilterGames(filter).CountAsync();
    }

    private IQueryable<Game> FilterGames(string? filter)
    {
        if (filter is null)
        {
            return _dbContext.Games;
        }

        return _dbContext.Games
                            .Where(game => game.Name.Contains(filter) || game.Genre.Contains(filter));
    }
}
