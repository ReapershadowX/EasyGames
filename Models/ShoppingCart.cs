using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesProject.Models
{
    public class ShoppingCart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        public int UserId { get; set; } // Foreign key to User

        [Required]
        public int StockId { get; set; } // Foreign key to Stock

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public DateTime DateAdded { get; set; } = DateTime.Now;

        // Navigation properties for EF Core (optional)
        [ForeignKey(nameof(UserId))]
        public virtual User? User { get; set; }

        [ForeignKey(nameof(StockId))]
        public virtual Stock? Stock { get; set; }
    }
}
