namespace GameHub.Services
{
    using GameHub.Data.Entities;
    using GameHub.Repositories.Interfaces;
    using GameHub.Services.Interfaces;

    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;

        public GameService(IGameRepository gameRepository)
        {
            _gameRepository = gameRepository;
        }

        public async Task<IEnumerable<Game>> GetAllGamesAsync(int pageNumber = 1, int pageSize = 10)
        {
            return await _gameRepository.GetPagedAsync(pageNumber, pageSize);
        }

        public async Task<Game> GetGameByIdAsync(Guid id)
        {
            return await _gameRepository.GetByIdAsync(id);
        }

        public async Task<Game> CreateGameAsync(Game game)
        {
            if (await _gameRepository.IsGameTitleExistsAsync(game.Title))
            {
                throw new InvalidOperationException($"A game with the name '{game.Title}' already exists.");
            }
            else
            {
                game.ID = Guid.NewGuid(); // Generate unique ID for the game
                return await _gameRepository.AddAsync(game);
            }
        }

        public async Task<bool> UpdateGameAsync(Game game)
        {
            var existingGame = await _gameRepository.GetByIdAsync(game.ID);
            if (existingGame == null)
                return false;

            if (await _gameRepository.IsGameTitleExistsAsync(game.Title, game.ID))
            {
                throw new InvalidOperationException($"A game with the name '{game.Title}' already exists.");
            }
            else
            {
                // Update fields
                existingGame.Title = game.Title;
                existingGame.Genre = game.Genre;
                existingGame.Description = game.Description;
                existingGame.Price = game.Price;
                existingGame.ReleaseDate = game.ReleaseDate;
                existingGame.StockQuantity = game.StockQuantity;

                return await _gameRepository.UpdateAsync(existingGame);
            }
        }

        public async Task<bool> DeleteGameAsync(Guid id)
        {
            return await _gameRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Game>> GetGamesByGenreAsync(string genre)
        {
            return await _gameRepository.GetGamesByGenreAsync(genre);
        }
    }
}
