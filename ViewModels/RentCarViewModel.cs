using System.ComponentModel.DataAnnotations;

namespace CarRentalSystem.ViewModels
{
    public class RentCarViewModel
    {
        public int CarId { get; set; }
        public string CarInfo { get; set; } = string.Empty;
        public decimal PricePerDay { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End date is required")]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        public int TotalDays { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
