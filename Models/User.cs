using System;
using System.ComponentModel.DataAnnotations;

namespace EasyGamesProject.Models
{
    public enum UserRole
    {
        Admin,
        Proprietor,
        Customer
    }

    public enum UserTier
    {
        None,
        Bronze,
        Silver,
        Gold,
        Platinum
    }

    public class User
    {
        [Key]
        public int UserId { get; set; }

        [Required(ErrorMessage = "First name is required.")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required.")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email address is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address format.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        public string Password { get; set; } = string.Empty;

        [Phone]
        [StringLength(15)]
        public string? PhoneNumber { get; set; }

        [Required]
        public UserRole Role { get; set; } = UserRole.Customer;

        // Tier only applies to Customers
        public UserTier? Tier { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        // Proprietor navigation
        public ICollection<Shop> Shops { get; set; } = new List<Shop>();
    }
}
