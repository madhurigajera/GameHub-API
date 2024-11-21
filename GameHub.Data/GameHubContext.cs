namespace GameHub.Data
{
    using GameHub.Data.Entities;
    using Microsoft.EntityFrameworkCore;
    public class GameHubContext : DbContext
    {
        public GameHubContext(DbContextOptions<GameHubContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        public DbSet<Game> Games { get; set; }
    }

   
}
