using EasyGamesProject.Models;
using System.ComponentModel.DataAnnotations;

public class StockImage
{
    [Key]
    public int ImageId { get; set; }

    [Required]
    public int StockId { get; set; }  // Foreign key to Stock

    public Stock? Stock { get; set; }

    [Required]
    public string ImageUrl { get; set; } = string.Empty;  // Path or URL to image file

    public string? Description { get; set; }  // Optional
}
