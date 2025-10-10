using CarRentalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Models
{
    public class Car : BaseEntity
    {
        [Required]
        [StringLength(100)]
        public string Brand { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Model { get; set; } = string.Empty;

        [Required]
        public int Year { get; set; }

        [Required]
        [StringLength(20)]
        public string LicensePlate { get; set; } = string.Empty;

        [Required]
        [Range(0.01, double.MaxValue)]
        public decimal PricePerDay { get; set; }

        [Required]
        public CarStatus Status { get; set; } = CarStatus.Available;

        public string? ImageUrl { get; set; }

        // Navigation property
        public virtual ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }

    public enum CarStatus
    {
        Available,
        Rented
    }
}
