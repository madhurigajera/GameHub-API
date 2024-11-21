
# GameHub API

The **GameHub API** is a .NET Core application that provides an API for managing games. It supports multiple database configurations based on the environment and user preferences. You can choose between an **In-Memory Database** for testing and lightweight development or a **SQL Server** database for production or persistent storage.

---

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Cloning the Repository](#cloning-the-repository)
3. [Running the Project Locally](#running-the-project-locally)
4. [Environment Setup](#environment-setup)
5. [Database Configuration](#database-configuration)
6. [Running Migrations](#running-migrations)
7. [Testing](#testing)

---

## Prerequisites

Before running the project locally, ensure you have the following installed:

- [Visual Studio](https://visualstudio.microsoft.com/) (or your preferred C# IDE).
- [.NET SDK](https://dotnet.microsoft.com/download/dotnet) version 6.0 or later.
- SQL Server (if you intend to use SQL Server as the database).

---

## Cloning the Repository

To clone the repository to your local machine:

```bash
git clone https://github.com/madhurigajera/GameHub-API.git
cd gamehub-api
```

---

## Running the Project Locally

Follow the steps below to run the project locally:

### Step 1: Open the Project

- Clone the repository to your local machine if not already done (refer to the [Cloning the Repository](#cloning-the-repository) section).
- Open the project in your preferred IDE (e.g., **Visual Studio** or **VS Code**).

### Step 2: Restore Dependencies

Run the following command to restore all required dependencies:

```bash
dotnet restore
```

### Step 3: Build the Project

Run the following command to build the project:

```bash
dotnet build
```

### Step 4: Set the Environment Using `launchSettings.json`

To configure the environment, update the `ASPNETCORE_ENVIRONMENT` variable in the `launchSettings.json` file, located in the `Properties` folder of the project.

#### Example Configuration in `launchSettings.json`:

```json
{
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "Project": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    }
  }
}
```

Ensure the correct environment (`Development`, `Staging`, or `Production`) is set.

### Step 5: Run the Project

Start the application by running the following command:

```bash
dotnet run
```

The application will start, and the API will be available at `http://localhost:5000` by default (or another port as configured).

### Step 6: Test the API Locally

The API includes a Swagger interface for exploring and testing endpoints interactively. Once the application is running, you can access Swagger by navigating to:

```plaintext
http://localhost:5000/swagger
```

Swagger provides a user-friendly UI where you can:

- View available endpoints.
- Test API endpoints with different parameters and request bodies.
- View request and response details.

#### Steps to Use Swagger:

1. Start the application using the `dotnet run` command.
2. Open your web browser and go to:

   ```plaintext
   http://localhost:5000/swagger
   ```

3. Use the interface to explore and test the endpoints.
   - Expand an endpoint to view its details.
   - Use the **Try it out** button to test the endpoint.

---

#### Example Request via Swagger:

1. Navigate to the **GET /api/games** endpoint.
2. Click **Try it out**.
3. Click **Execute** to send the request and view the response.

Swagger makes it easy to test the API without needing external tools like Postman or curl.

---

## Environment Setup

The application supports multiple environments such as `Development`, and `Production`. You can set the environment using the `launchSettings.json` file or by using environment variables.

### Example Configuration in `launchSettings.json`:

```json
{
  "profiles": {
    "IIS Express": {
      "commandName": "IISExpress",
      "launchBrowser": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Development"
      }
    },
    "Project": {
      "commandName": "Project",
      "dotnetRunMessages": true,
      "environmentVariables": {
        "ASPNETCORE_ENVIRONMENT": "Production"
      }
    }
  }
}
```

Alternatively, set the environment at runtime using PowerShell:

```bash
$env:ASPNETCORE_ENVIRONMENT = "Production"
```

---

## Database Configuration

The application supports both **In-Memory Database** and **SQL Server**. You can configure the database by updating the `appsettings.json` or `appsettings.{Environment}.json` files.

### Using In-Memory Database:

Set the `UseOnlyInMemoryDatabase` key to `true` and provide In memory DB name in the `appsettings.json` file:

```json
{
  "DatabaseConfig": {
    "UseOnlyInMemoryDatabase": true,
    "ConnectionString": "In Memory DB Name"
  }
}
```

### Using SQL Server:

Set the `UseOnlyInMemoryDatabase` key to `false` and provide the SQL Server connection string:

```json
{
  "DatabaseConfig": {
    "UseOnlyInMemoryDatabase": false,
    "ConnectionString": "Server=YOUR_SERVER_NAME;Database=GameHubDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;"
  }
}
```

---

## Running Migrations

The application follows a **Code-First** approach for database management. This means that you do not need to manually run migration scripts. 

### How Migrations Work in the Application

When the application starts, it automatically checks if the database exists. If the database does not exist, it will run the necessary migrations to create the database schema based on the latest model.

The application uses **Entity Framework Core** to apply migrations automatically on startup.

### Manual Migration (Optional)

Although the application runs migrations automatically, if you prefer to run migrations manually, you can still do so by following these steps:

1. Add the migration:

   ```bash
   dotnet ef migrations add InitialCreate
   ```

2. Apply the migration to the database:

   ```bash
   dotnet ef database update
   ```

   This will manually apply the migration to your configured database (e.g., SQL Server, if configured).

---

## Testing

This application includes unit tests for two main areas:

1. **API Controller Testing**: Verifying the API controllers' responses and their interaction with the underlying service layer.
2. **Service Layer Testing**: Ensuring that the business logic in the service layer works correctly, including validation, data processing, and interaction with the repository.

### 1. API Controller Testing

API Controller tests ensure that the HTTP endpoints are functioning as expected. These tests simulate real HTTP requests to the controller and verify that the API returns the correct responses.

#### Example Test:

- Test if the **GET /api/games** endpoint correctly returns a list of games.

```csharp
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
```

In this test, the `GameController` is tested by mocking the **IGameService** and verifying that the **GET /api/games** endpoint returns the expected results.

### 2. Service Layer Testing

Service layer tests focus on the logic within the service classes. These tests verify that the services interact with the repositories and data layer as expected and that the business logic is applied correctly.

#### Example Test:

- Test if the **CreateGame** method in the service correctly adds a new game.

```csharp
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
```

In this test, the service method `CreateGame` is tested by mocking the **IGameRepository** and verifying that the `AddGameAsync` method is called with the correct parameters.

---

### Running Tests

To run all tests:

1. Navigate to the test project directory:

   ```bash
   cd GameHub.UnitTests
   ```

2. Run the tests:

   ```bash
   dotnet test
   ```

This will run both the **API controller** tests and **service layer** tests, ensuring that your API and business logic behave correctly.

### Test Coverage

- **Controller Tests**: Ensure that the API endpoints are correctly wired up and return the expected responses.
- **Service Tests**: Ensure that the business logic and interactions with the repository are correct and perform as expected.

By running these tests, you can be confident that both the API and the service layer are functioning correctly and independently.

---
