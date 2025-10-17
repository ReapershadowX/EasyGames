using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesProject.Models
{
    public class ShopStock
    {
        [Key]
        public int ShopStockId { get; set; }  // Primary key

        [Required]
        public int ShopId { get; set; }

        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }  // Nullable navigation property

        [Required]
        public int StockId { get; set; }

        [ForeignKey("StockId")]
        public Stock? Stock { get; set; }  // Nullable navigation property

        [Required]
        public int Quantity { get; set; }  // Number of stock items in this shop

        // New fields to inherit from Stock (owner)
        public string Source { get; set; } = string.Empty;

        public decimal BuyPrice { get; set; }

        public decimal SellPrice { get; set; }
    }
}
