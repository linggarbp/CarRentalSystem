using CarRentalSystem.Models;
using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.Models
{
    public class Rental : BaseEntity
    {
        [Required]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public int CarId { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public int TotalDays { get; set; }

        [Required]
        public decimal TotalPrice { get; set; }

        [Required]
        public RentalStatus Status { get; set; } = RentalStatus.Active;

        // Navigation properties
        public virtual ApplicationUser User { get; set; } = null!;
        public virtual Car Car { get; set; } = null!;
    }

    public enum RentalStatus
    {
        Active,
        Completed
    }
}
