using System.ComponentModel.DataAnnotations;

namespace EasyGamesProject.Models
{
    public class Shop
    {
        [Key]
        public int ShopId { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(255)]
        public string Location { get; set; } = string.Empty;

        [Required]
        public int ProprietorId { get; set; }  // User ID of shop proprietor
    }
}
