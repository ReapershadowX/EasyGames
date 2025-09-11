using System;
using System.ComponentModel.DataAnnotations;

namespace EasyGamesProject.Models
{
    public class Stock
    {
        [Key]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty; // Initialized to satisfy non-nullable constraint

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty; // Initialized to satisfy non-nullable constraint; e.g., Books, Games, Toys

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }

        // Description is optional and initialized to empty string to support non-nullability
        public string Description { get; set; } = string.Empty;

        // Record creation timestamp of the stock entry, defaulting to now
        public DateTime CreatedDate { get; set; } = DateTime.Now;
    }
}
