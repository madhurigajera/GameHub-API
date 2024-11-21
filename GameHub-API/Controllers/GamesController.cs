namespace GameHub_API.Controllers
{
    using GameHub.Data.Entities;
    using GameHub.Services.Interfaces;
    using Microsoft.AspNetCore.Mvc;

    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly IGameService _gameService;
        private readonly ILogger<GamesController> _logger;

        public GamesController(IGameService gameService, ILogger<GamesController> logger)
        {
            _gameService = gameService;
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a paginated list of games.
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <returns>A paginated list of games.</returns>
        /// <response code="200">Returns the list of games.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)] // When the list of games is retrieved successfully
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // When an internal server error occurs
        public async Task<IActionResult> GetGames([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var games = await _gameService.GetAllGamesAsync(pageNumber, pageSize);
                return Ok(games);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the games");
                throw;
            }
            
        }

        /// <summary>
        /// Retrieves the details of a game by its unique ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>The game if found; otherwise, a 404 error.</returns>
        /// <response code="200">Returns the game details.</response>
        /// <response code="404">If the game is not found.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpGet("game/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Game))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetGame(Guid id)
        {
            try
            {
                var game = await _gameService.GetGameByIdAsync(id);
                if (game == null)
                {
                    _logger.LogWarning("Game with ID {GameId} was not found.", id);
                    return NotFound();
                }

                return Ok(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the game with ID {GameId}.", id);
                throw; 
            }
        }

        /// <summary>
        /// Retrieves a list of games that match the specified genre.
        /// </summary>
        /// <param name="genre"></param>
        /// <returns>A list of games in the specified genre.</returns>
        /// <response code="200">Returns the list of games in the genre.</response>
        /// <response code="404">If no games are found in the specified genre.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpGet("genre/{genre}")]
        [ProducesResponseType(StatusCodes.Status200OK)] // When the games in the specified genre are found
        [ProducesResponseType(StatusCodes.Status404NotFound)] // When no games are found for the specified genre
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // When an internal server error occurs
        public async Task<IActionResult> GetGameByGenre(string genre)
        {
            try
            {
                var game = await _gameService.GetGamesByGenreAsync(genre);
                if (game == null)
                {
                    _logger.LogWarning("Game with Genre {genre} was not found.", genre);
                    return NotFound();
                }

                return Ok(game);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while retrieving the game with genre {genre}.", genre);
                throw;
            }
        }

        /// <summary>
        /// Creates a new game in the system.
        /// </summary>
        /// <param name="game"></param>
        /// <returns>The created game with its unique ID.</returns>
        /// <response code="201">Returns the newly created game.</response>
        /// <response code="400">If the game data is invalid.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)] // When the game is created successfully
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // When the provided game data is invalid
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // When an internal server error occurs
        public async Task<IActionResult> CreateGame([FromBody] Game game)
        {
            try
            {
                var createdGame = await _gameService.CreateGameAsync(game);
                return CreatedAtAction(nameof(GetGame), new { id = createdGame.ID }, createdGame);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while creating a game with title: {Title}.", game.Title);
                throw; 
            }
          
        }

        /// <summary>
        /// Updates an existing game by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="game"></param>
        /// <returns>No content if the update is successful; otherwise, an error response.</returns>
        /// <response code="204">If the game is updated successfully.</response>
        /// <response code="400">If the ID in the URL does not match the ID in the game data.</response>
        /// <response code="404">If the game with the specified ID is not found.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // When the game is updated successfully
        [ProducesResponseType(StatusCodes.Status400BadRequest)] // When the provided ID doesn't match the game ID
        [ProducesResponseType(StatusCodes.Status404NotFound)] // When the game with the given ID is not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // When an internal server error occurs
        public async Task<IActionResult> UpdateGame(Guid id, [FromBody] Game game)
        {
            try
            {
                if (id != game.ID)
                {
                    _logger.LogWarning("Mismatched game ID in update request. URL ID: {UrlId}, Game ID: {GameId}.", id, game.ID);
                    return BadRequest();
                }

                _logger.LogInformation("Attempting to update game with ID: {GameId}.", id);

                var success = await _gameService.UpdateGameAsync(game);
                if (!success)
                {
                    _logger.LogWarning("Game with ID: {GameId} not found for update.", id);
                    return NotFound();
                }

                _logger.LogInformation("Game with ID: {GameId} updated successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating the game with ID: {GameId}.", id);
                throw; 
            }
        }

        /// <summary>
        /// Deletes a game by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>No content if the deletion is successful; otherwise, an error response.</returns>
        /// <response code="204">If the game is deleted successfully.</response>
        /// <response code="404">If the game with the specified ID is not found.</response>
        /// <response code="500">If an internal error occurs.</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)] // When the game is deleted successfully
        [ProducesResponseType(StatusCodes.Status404NotFound)] // When the game with the given ID is not found
        [ProducesResponseType(StatusCodes.Status500InternalServerError)] // When an internal server error occurs
        public async Task<IActionResult> DeleteGame(Guid id)
        {
            try
            {
                _logger.LogInformation("Attempting to delete game with ID: {GameId}.", id);

                var success = await _gameService.DeleteGameAsync(id);
                if (!success)
                {
                    _logger.LogWarning("Game with ID: {GameId} not found for deletion.", id);
                    return NotFound();
                }

                _logger.LogInformation("Game with ID: {GameId} deleted successfully.", id);
                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting the game with ID: {GameId}.", id);
                throw; 
            }
        }
    }
}
