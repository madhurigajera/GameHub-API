namespace GameHub.Repositories
{
    using GameHub.Data;
    using GameHub.Data.Entities;
    using GameHub.Repositories.Interfaces;
    using Microsoft.EntityFrameworkCore;
    public class GameRepository : GenericRepository<Game>, IGameRepository
    {
        private readonly GameHubContext _context;

        public GameRepository(GameHubContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Game>> GetGamesByGenreAsync(string genre)
        {
            return await _context.Set<Game>()
                                 .Where(g => g.Genre.ToLower() == genre.ToLower())
                                 .ToListAsync();
        }
        public async Task<bool> IsGameTitleExistsAsync(string name, Guid? gameId = null)
        {
            return await _context.Games.AnyAsync(g => g.Title == name && (!gameId.HasValue || g.ID != gameId));
        }
    }
}
