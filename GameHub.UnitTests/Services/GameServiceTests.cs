namespace GameHub.UnitTests.Services
{
    using GameHub.Data.Entities;
    using Moq;

    public class GameServiceTests
    {
        private readonly TestSetup _testSetup;

        public GameServiceTests()
        {
            _testSetup = new TestSetup();
        }

        [Fact]
        public async Task GetAllGamesAsync_ReturnsPagedGames()
        {
            // Arrange
            int pageNumber = 1;
            int pageSize = 10;
            var games = new List<Game> { _testSetup.SampleGame, _testSetup.ErrorGame };
            _testSetup.MockGameRepository.Setup(r => r.GetPagedAsync(pageNumber, pageSize)).ReturnsAsync(games);

            // Act
            var result = await _testSetup.GameService.GetAllGamesAsync(pageNumber, pageSize);

            // Assert
            Assert.Equal(2, result.Count());
            _testSetup.MockGameRepository.Verify(r => r.GetPagedAsync(pageNumber, pageSize), Times.Once);
        }

        [Fact]
        public async Task GetGameByIdAsync_ReturnsGame_WhenGameExists()
        {
            // Arrange
            
            var game = _testSetup.SampleGame;
            var gameId = game.ID;

            _testSetup.MockGameRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync(game);

            // Act
            var result = await _testSetup.GameService.GetGameByIdAsync(gameId);

            // Assert
            Assert.Equal(gameId, result.ID);
            _testSetup.MockGameRepository.Verify(r => r.GetByIdAsync(gameId), Times.Once);
        }

        [Fact]
        public async Task GetGameByIdAsync_ReturnsNull_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _testSetup.MockGameRepository.Setup(r => r.GetByIdAsync(gameId)).ReturnsAsync((Game)null);

            // Act
            var result = await _testSetup.GameService.GetGameByIdAsync(gameId);

            // Assert
            Assert.Null(result);
            _testSetup.MockGameRepository.Verify(r => r.GetByIdAsync(gameId), Times.Once);
        }

        [Fact]
        public async Task CreateGameAsync_CreatesGameSuccessfully()
        {
            // Arrange
            var game = _testSetup.SampleGame;
            _testSetup.MockGameRepository.Setup(r => r.AddAsync(game)).ReturnsAsync(game);

            // Act
            var result = await _testSetup.GameService.CreateGameAsync(game);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(game.Title, result.Title);
            _testSetup.MockGameRepository.Verify(r => r.AddAsync(game), Times.Once);
        }

        [Fact]
        public async Task CreateGame_ShouldThrowException_IfDuplicateGameNameExists()
        {
            // Arrange
            var game = _testSetup.SampleGame;
            var duplicateGame = _testSetup.DuplicateGame;
            var newGame = new Game { ID = game.ID, Title = "Duplicate Name", Description = "Create", Genre = "", Price = 20 };

            _testSetup.MockGameRepository.Setup(repo => repo.GetByIdAsync(game.ID)).ReturnsAsync(game);
            _testSetup.MockGameRepository.Setup(repo => repo.IsGameTitleExistsAsync("Duplicate Name",null)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _testSetup.GameService.CreateGameAsync(newGame)
            );

            Assert.Equal("A game with the name 'Duplicate Name' already exists.", exception.Message);
        }

        [Fact]
        public async Task UpdateGameAsync_ReturnsFalse_WhenGameDoesNotExist()
        {
            // Arrange
            var game = _testSetup.SampleGame;
            _testSetup.MockGameRepository.Setup(r => r.GetByIdAsync(game.ID)).ReturnsAsync((Game)null);

            // Act
            var result = await _testSetup.GameService.UpdateGameAsync(game);

            // Assert
            Assert.False(result);
            _testSetup.MockGameRepository.Verify(r => r.GetByIdAsync(game.ID), Times.Once);
        }

        [Fact]
        public async Task UpdateGameAsync_UpdatesGameSuccessfully_WhenGameExists()
        {
            // Arrange
            var game = new Game { ID = Guid.NewGuid(), Title = "Updated Game" , Genre = ""};
            var existingGame = new Game { ID = game.ID, Title = "Old Game", Genre = "" };
            _testSetup.MockGameRepository.Setup(r => r.GetByIdAsync(game.ID)).ReturnsAsync(existingGame);
            _testSetup.MockGameRepository.Setup(r => r.UpdateAsync(existingGame)).ReturnsAsync(true);

            // Act
            var result = await _testSetup.GameService.UpdateGameAsync(game);

            // Assert
            Assert.True(result);
            Assert.Equal(game.Title, existingGame.Title);  // Ensure that the title is updated
            _testSetup.MockGameRepository.Verify(r => r.GetByIdAsync(game.ID), Times.Once);
            _testSetup.MockGameRepository.Verify(r => r.UpdateAsync(existingGame), Times.Once);
        }

        [Fact]
        public async Task UpdateGame_ShouldThrowException_IfDuplicateGameNameExists()
        {
            // Arrange
            var game = _testSetup.SampleGame;
            var duplicateGame = _testSetup.DuplicateGame;
            var updatedGame = new Game { ID = game.ID, Title = "Duplicate Name", Description = "Updated", Genre = "", Price = 20 };

            _testSetup.MockGameRepository.Setup(repo => repo.GetByIdAsync(game.ID)).ReturnsAsync(game);
            _testSetup.MockGameRepository.Setup(repo => repo.IsGameTitleExistsAsync("Duplicate Name", game.ID)).ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _testSetup.GameService.UpdateGameAsync(updatedGame)
            );

            Assert.Equal("A game with the name 'Duplicate Name' already exists.", exception.Message);
        }

        [Fact]
        public async Task DeleteGameAsync_ReturnsFalse_WhenGameDoesNotExist()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _testSetup.MockGameRepository.Setup(r => r.DeleteAsync(gameId)).ReturnsAsync(false);

            // Act
            var result = await _testSetup.GameService.DeleteGameAsync(gameId);

            // Assert
            Assert.False(result);
            _testSetup.MockGameRepository.Verify(r => r.DeleteAsync(gameId), Times.Once);
        }

        [Fact]
        public async Task DeleteGameAsync_ReturnsTrue_WhenGameIsDeleted()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _testSetup.MockGameRepository.Setup(r => r.DeleteAsync(gameId)).ReturnsAsync(true);

            // Act
            var result = await _testSetup.GameService.DeleteGameAsync(gameId);

            // Assert
            Assert.True(result);
            _testSetup.MockGameRepository.Verify(r => r.DeleteAsync(gameId), Times.Once);
        }

        [Fact]
        public async Task GetGamesByGenreAsync_ReturnsGames_WhenGenreExists()
        {
            // Arrange
            var genre = "Action";
            var games = new List<Game> { _testSetup.SampleGame, _testSetup.ErrorGame };
            _testSetup.MockGameRepository.Setup(r => r.GetGamesByGenreAsync(genre)).ReturnsAsync(games);

            // Act
            var result = await _testSetup.GameService.GetGamesByGenreAsync(genre);

            // Assert
            Assert.Equal(2, result.Count());
            _testSetup.MockGameRepository.Verify(r => r.GetGamesByGenreAsync(genre), Times.Once);
        }

        [Fact]
        public async Task GetGamesByGenreAsync_ReturnsEmpty_WhenGenreDoesNotExist()
        {
            // Arrange
            var genre = "NonExistentGenre";
            var games = new List<Game>();
            _testSetup.MockGameRepository.Setup(r => r.GetGamesByGenreAsync(genre)).ReturnsAsync(games);

            // Act
            var result = await _testSetup.GameService.GetGamesByGenreAsync(genre);

            // Assert
            Assert.Empty(result);
            _testSetup.MockGameRepository.Verify(r => r.GetGamesByGenreAsync(genre), Times.Once);
        }
    }

}
