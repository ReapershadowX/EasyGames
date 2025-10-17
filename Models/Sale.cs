// Models/Sale.cs
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EasyGamesProject.Models
{
    public class Sale
    {
        [Key]
        public int SaleId { get; set; }

        [Required]
        public int ShopId { get; set; }

        [ForeignKey("ShopId")]
        public Shop? Shop { get; set; }

        public int? UserId { get; set; } // Nullable for guest purchases

        [ForeignKey("UserId")]
        public User? User { get; set; }

        [Phone]
        [StringLength(15)]
        public string? CustomerPhone { get; set; }

        [Required]
        public int StockId { get; set; }

        [ForeignKey("StockId")]
        public Stock? Stock { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal DiscountAmount { get; set; } = 0;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }

        [Required]
        public DateTime SaleDate { get; set; } = DateTime.Now;

        [StringLength(50)]
        public string SaleType { get; set; } = "POS"; // POS or Online

        public string? Notes { get; set; }
    }
}
