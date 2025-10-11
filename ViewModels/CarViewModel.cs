using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class CarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
        [Display(Name = "Brand")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
        [Display(Name = "Model")]
        public string Model { get; set; }

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        [Display(Name = "Year")]
        public int Year { get; set; }

        [Required(ErrorMessage = "License plate is required")]
        [StringLength(20, ErrorMessage = "License plate cannot exceed 20 characters")]
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; }

        [Required(ErrorMessage = "Price per day is required")]
        [Range(50000, 5000000, ErrorMessage = "Price must be between Rp 50,000 and Rp 5,000,000")]
        [Display(Name = "Price Per Day (Rp)")]
        public decimal PricePerDay { get; set; }

        [Display(Name = "Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }

        public int PricePerDayAsInt => (int)PricePerDay;

        public CarViewModel()
        {
            Brand = string.Empty;
            Model = string.Empty;
            LicensePlate = string.Empty;
        }
    }
}
