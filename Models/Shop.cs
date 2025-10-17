using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public int ProprietorId { get; set; }  // Foreign key

        [ForeignKey("ProprietorId")]
        public User? Proprietor { get; set; } 
    }
}
