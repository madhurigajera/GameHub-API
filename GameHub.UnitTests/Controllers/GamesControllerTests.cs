namespace GameHub.UnitTests.Controllers
{
    using GameHub.Data.Entities;
    using GameHub.Services.Interfaces;
    using GameHub_API.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Moq;

    public class GamesControllerTests
    {
        private readonly TestSetup _testSetup;

        public GamesControllerTests()
        {
            _testSetup = new TestSetup();
        }

        [Fact]
        public async Task GetAllGames_ReturnsOkResult_WithGameList()
        {
            // Arrange
            var games = new List<Game>
            {
                _testSetup.SampleGame,
                _testSetup.DuplicateGame
            };
            _testSetup.MockGameService.Setup(service => service.GetAllGamesAsync(1,10))
                .ReturnsAsync(games);

            // Act
            var result = await _testSetup.Controller.GetGames();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedGames = Assert.IsAssignableFrom<IEnumerable<Game>>(okResult.Value);
            Assert.Equal(2, returnedGames.Count());
        }

        [Fact]
        public async Task CreateGame_ReturnsCreatedAtActionResult_WhenGameIsCreated()
        {
            // Arrange
            var game = _testSetup.SampleGame;
            _testSetup.MockGameService.Setup(s => s.CreateGameAsync(game)).ReturnsAsync(game);

            // Act
            var result = await _testSetup.Controller.CreateGame(game);

            // Assert
            var actionResult = Assert.IsType<CreatedAtActionResult>(result);
            Assert.Equal(nameof(_testSetup.Controller.GetGame), actionResult.ActionName);
            Assert.Equal(game.ID, ((Game)actionResult.Value).ID);
        }

        [Fact]
        public async Task CreateGame_LogsErrorAndThrows_WhenExceptionOccurs()
        {
            // Arrange
            var game = _testSetup.SampleGame;
            var exceptionMessage = "Database error";
            _testSetup.MockGameService.Setup(s => s.CreateGameAsync(game)).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _testSetup.Controller.CreateGame(game));
            Assert.Equal(exceptionMessage, exception.Message);

            _testSetup.MockLogger.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while creating a game with title: {game.Title}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);

        }

        [Fact]
        public async Task UpdateGame_ReturnsNoContent_WhenGameIsUpdated()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _testSetup.SampleGame;
            _testSetup.MockGameService.Setup(s => s.UpdateGameAsync(game)).ReturnsAsync(true);

            // Act
            var result = await _testSetup.Controller.UpdateGame(game.ID, game);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task UpdateGame_ReturnsBadRequest_WhenIdsDoNotMatch()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var game = _testSetup.MismatchGame;

            // Act
            var result = await _testSetup.Controller.UpdateGame(gameId, game);

            // Assert
            var badRequestResult = Assert.IsType<BadRequestResult>(result);
            _testSetup.MockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains("Mismatched game ID")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateGame_ReturnsNotFound_WhenGameIsNotFound()
        {
            // Arrange
            //var game = _testSetup.NonExistGame;
            //_testSetup.MockGameService.Setup(s => s.UpdateGameAsync(game)).ReturnsAsync(false);

            //// Act
            //var result = await _testSetup.Controller.UpdateGame(game.ID, game);

            //// Assert
            //Assert.IsType<NotFoundResult>(result);
            //_testSetup.MockLogger.Verify(
            //    log => log.LogWarning(It.Is<string>(msg => msg.Contains("not found for update")), game.ID),
            //    Times.Once);

            // Arrange
            var game = _testSetup.NonExistGame;
            _testSetup.MockGameService.Setup(s => s.UpdateGameAsync(game)).ReturnsAsync(false);

            // Act
            var result = await _testSetup.Controller.UpdateGame(game.ID, game);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _testSetup.MockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Game with ID: {game.ID} not found for update")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task UpdateGame_LogsErrorAndThrows_WhenExceptionOccurs()
        {
            // Arrange
            var game = _testSetup.ErrorGame;
            _testSetup.MockGameService.Setup(s => s.UpdateGameAsync(game)).ThrowsAsync(new Exception("Database error"));

            // Act & Assert
            await Assert.ThrowsAsync<Exception>(() => _testSetup.Controller.UpdateGame(game.ID, game));
            _testSetup.MockLogger.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while updating the game with ID: {game.ID}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteGame_ReturnsNoContent_WhenGameIsDeleted()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _testSetup.MockGameService.Setup(s => s.DeleteGameAsync(gameId)).ReturnsAsync(true);

            // Act
            var result = await _testSetup.Controller.DeleteGame(gameId);

            // Assert
            Assert.IsType<NoContentResult>(result);
        }

        [Fact]
        public async Task DeleteGame_ReturnsNotFound_WhenGameIsNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            _testSetup.MockGameService.Setup(s => s.DeleteGameAsync(gameId)).ReturnsAsync(false);

            // Act
            var result = await _testSetup.Controller.DeleteGame(gameId);

            // Assert
            Assert.IsType<NotFoundResult>(result);
            _testSetup.MockLogger.Verify(log => log.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"Game with ID: {gameId} not found for deletion")),
                null,
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }

        [Fact]
        public async Task DeleteGame_LogsErrorAndThrows_WhenExceptionOccurs()
        {
            // Arrange
            var gameId = Guid.NewGuid();
            var exceptionMessage = "Database error";
            _testSetup.MockGameService.Setup(s => s.DeleteGameAsync(gameId)).ThrowsAsync(new Exception(exceptionMessage));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<Exception>(() => _testSetup.Controller.DeleteGame(gameId));
            Assert.Equal(exceptionMessage, exception.Message);

            _testSetup.MockLogger.Verify(log => log.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => v.ToString().Contains($"An error occurred while deleting the game with ID: {gameId}")),
                It.IsAny<Exception>(),
                It.IsAny<Func<It.IsAnyType, Exception, string>>()), Times.Once);
        }
    }
}
