using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
using System.Text.Json;


namespace EasyGamesProject.ViewModels
{
    public class POSViewModel
    {
        [Display(Name = "Customer Phone (Optional)")]
        [Phone]
        public string? CustomerPhone { get; set; }

        public string? CustomerName { get; set; }
        public string? CustomerTier { get; set; }
        public decimal TierDiscount { get; set; }

        [Required]
        [Display(Name = "Select Item")]
        public int SelectedStockId { get; set; }

        [Required]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        public List<SelectListItem>? AvailableStock { get; set; }

        // For displaying selected item details
        public string? SelectedItemName { get; set; }
        public decimal SelectedItemPrice { get; set; }
        public int SelectedItemAvailable { get; set; }

        public decimal Subtotal { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal Total { get; set; }

        public List<POSCartItem> CartItems { get; set; } = new List<POSCartItem>();

        // JSON serialization of cart for posting
        public string? CartJson { get; set; }
    }

    public class POSCartItem
    {
        public int StockId { get; set; }
        public string? ItemName { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Subtotal { get; set; }
    }
}