using CarRentalSystem.Models;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        public DateTime CreatedDate { get; set; } = DateTime.Now;

        public string FullName => $"{FirstName} {LastName}";

        // Navigation properties
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}
