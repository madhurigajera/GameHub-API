namespace GameHub.UnitTests
{
    using GameHub.Data.Entities;
    using GameHub.Repositories.Interfaces;
    using GameHub.Services;
    using GameHub.Services.Interfaces;
    using GameHub_API.Controllers;
    using Microsoft.Extensions.Logging;
    using Moq;

    public class TestSetup
    {
        public Mock<IGameService> MockGameService { get; private set; }
        
        public Mock<ILogger<GamesController>> MockLogger { get; private set; }

        public Mock<IGameRepository> MockGameRepository { get; private set; }
        
        public GamesController Controller { get; private set; }

        public GameService GameService { get; private set; }

        public Game SampleGame { get; private set; }
      
        public Game DuplicateGame { get; private set; }

        public Game MismatchGame { get; private set; }
        
        public Game NonExistGame { get; private set; }
       
        public Game ErrorGame { get; private set; }


        public TestSetup()
        {
            // Initialize sample data
            InitializeGameData();
            // Initialize mocks
            MockGameService = new Mock<IGameService>();
            MockLogger = new Mock<ILogger<GamesController>>();
            MockGameRepository = new Mock<IGameRepository>();

            // Initialize the controller with mocks
            Controller = new GamesController(MockGameService.Object, MockLogger.Object);
            GameService = new GameService(MockGameRepository.Object);

        }
        private void InitializeGameData()
        {
            // Sample game data for testing
            SampleGame = CreateGame("Sample Game", 49.99M);
            DuplicateGame = CreateGame("Duplicate Name", 49.99M); // Duplicate for conflict testing
            MismatchGame = CreateGame("Mismatch Game", 0); // A game with missing data for testing errors
            NonExistGame = CreateGame("Non-existent Game", 0); // For non-existing game scenarios
            ErrorGame = CreateGame("Error Game", 0); // For error scenarios
        }
        private Game CreateGame(string title, decimal price)
        {
            return new Game
            {
                ID = Guid.NewGuid(),
                Title = title,
                Genre = "Action",
                Description = "A test game description",
                Price = price,
                ReleaseDate = DateTime.Now,
                StockQuantity = 5
            };
        }
    }

}
