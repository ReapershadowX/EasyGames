using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesProject.Models
{
    public class Stock : IValidatableObject
    {
        [Key]
        public int StockId { get; set; }

        [Required(ErrorMessage = "Name is required.")]
        [StringLength(100, ErrorMessage = "Name cannot exceed 100 characters.")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Category is required.")]
        public string Category { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        public decimal SellPrice { get; set; } // renamed from Price for clarity

        [Required(ErrorMessage = "Buy price is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Buy price must be greater than 0")]
        public decimal BuyPrice { get; set; } // new field

        [Required(ErrorMessage = "Quantity is required.")]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity cannot be negative")]
        public int Quantity { get; set; }  // Owner's available stock quantity

        public string Source { get; set; } = string.Empty; // new field to track origin

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public ICollection<StockImage>? Images { get; set; } = new List<StockImage>();

        // Navigation property to relate to shop stocks
        public ICollection<ShopStock>? ShopStocks { get; set; }

        // Computed property for profit margin percentage
        public decimal ProfitMargin
        {
            get => SellPrice != 0 ? (SellPrice - BuyPrice) / SellPrice * 100 : 0;
        }

        // Computed available quantity not yet assigned to shops
        [NotMapped]
        public int AvailableQuantity => Quantity - (ShopStocks?.Sum(ss => ss.Quantity) ?? 0);

        // Custom validation logic
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (BuyPrice > SellPrice)
            {
                yield return new ValidationResult(
                    "Buy price cannot be greater than Sell price.",
                    new[] { nameof(BuyPrice), nameof(SellPrice) });
            }
        }
    }
}
