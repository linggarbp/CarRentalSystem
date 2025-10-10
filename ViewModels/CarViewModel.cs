using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class CarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Brand is required")]
        [StringLength(100, ErrorMessage = "Brand cannot exceed 100 characters")]
        [Display(Name = "Brand")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "Model is required")]
        [StringLength(100, ErrorMessage = "Model cannot exceed 100 characters")]
        [Display(Name = "Model")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "Year is required")]
        [Range(1900, 2030, ErrorMessage = "Year must be between 1900 and 2030")]
        [Display(Name = "Year")]
        public int Year { get; set; } = 2023; // SET DEFAULT VALUE

        [Required(ErrorMessage = "License plate is required")]
        [StringLength(20, ErrorMessage = "License plate cannot exceed 20 characters")]
        [Display(Name = "License Plate")]
        public string LicensePlate { get; set; } = string.Empty;

        [Required(ErrorMessage = "Price per day is required")]
        [Range(0.01, 999999.99, ErrorMessage = "Price must be between 0.01 and 999,999.99")]
        [Display(Name = "Price Per Day ($)")]
        public decimal PricePerDay { get; set; } = 50.00m; // SET DEFAULT VALUE

        [Display(Name = "Image URL")]
        [Url(ErrorMessage = "Please enter a valid URL")]
        public string? ImageUrl { get; set; }
    }
}
