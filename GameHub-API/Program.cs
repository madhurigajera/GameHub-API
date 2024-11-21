using GameHub.Data;
using GameHub.Repositories;
using GameHub.Repositories.Interfaces;
using GameHub.Services;
using GameHub.Services.Interfaces;
using GameHub_API.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);


// Add JSON configuration files in the correct order
builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables(); // Add environment variables last


// Add logging configuration
builder.Logging.ClearProviders(); // Clear default providers
builder.Logging.AddConsole();     // Add Console logging
builder.Logging.AddDebug();       // Add Debug logging
builder.Logging.SetMinimumLevel(LogLevel.Information);

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "GameHub API",
        Version = "v1",
        Description = "API for managing games",
    });
});

// Inject the connection string using Inmemory database
var connectionString = builder.Configuration.GetConnectionString("GameHubConnection");

if(connectionString != null)
{
    var useInMemory = builder.Configuration.GetSection("UseOnlyInMemoryDatabase");

    bool useInMemoryDB = true;

    if (useInMemory != null)
    {
        useInMemoryDB = bool.Parse(useInMemory.Value);
    }


    if (useInMemoryDB)
    {
        builder.Services.AddDbContext<GameHubContext>(context =>
                context.UseInMemoryDatabase(connectionString));

    }
    else
    {
        builder.Services.AddDbContext<GameHubContext>(c =>
                  c.UseSqlServer(connectionString));
    }
}

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Registering services and Repositories

builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<IGameService, GameService>();

var app = builder.Build();

// Run migrations during startup
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<GameHubContext>();
    context.Database.EnsureCreated(); // Applies migrations or initializes schema
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwagger();

app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "GameHub API v1");
});


app.Run();
