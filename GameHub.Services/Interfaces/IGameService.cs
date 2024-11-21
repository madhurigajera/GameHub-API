namespace GameHub.Services.Interfaces
{
    using GameHub.Data.Entities;

    public interface IGameService
    {
        Task<IEnumerable<Game>> GetAllGamesAsync(int pageNumber = 1, int pageSize = 10);
        Task<Game> GetGameByIdAsync(Guid id);
        Task<Game> CreateGameAsync(Game game);
        Task<bool> UpdateGameAsync(Game game);
        Task<bool> DeleteGameAsync(Guid id);
        Task<IEnumerable<Game>> GetGamesByGenreAsync(string genre);
    }
}
