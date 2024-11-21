namespace GameHub.Repositories.Interfaces
{
    using GameHub.Data.Entities;
    public interface IGameRepository : IGenericRepository<Game>
    {
        Task<IEnumerable<Game>> GetGamesByGenreAsync(string genre);
        Task<bool> IsGameTitleExistsAsync(string name, Guid? gameId = null);
    }
}

