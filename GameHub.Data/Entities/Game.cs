namespace GameHub.Data.Entities
{
    using System.ComponentModel.DataAnnotations;

    public class Game
    {
        [Key]
        public Guid ID { get; set; } // Unique identifier for the game

        [Required]
        [StringLength(100)]
        public required string Title { get; set; } // Name of the game

        [Required]
        [StringLength(50)]
        public required string Genre { get; set; } // Genre of the game (e.g., Action, RPG)

        [StringLength(500)]
        public string? Description { get; set; } // Brief description of the game

        [Required]
        [Range(0.01, 10000.00)]
        public decimal Price { get; set; } // Price of the game in USD

        [Required]
        public DateTime ReleaseDate { get; set; } // Date when the game was released

        [Required]
        [Range(0, int.MaxValue)]
        public int StockQuantity { get; set; } // Quantity of the game in stock
    }
}
