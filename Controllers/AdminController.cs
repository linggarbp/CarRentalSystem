using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Services;

namespace CarRentalSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ICarRepository _carRepository;
        private readonly IRentalService _rentalService;

        public AdminController(ICarRepository carRepository, IRentalService rentalService)
        {
            _carRepository = carRepository;
            _rentalService = rentalService;
        }

        public async Task<IActionResult> Index()
        {
            var allCars = await _carRepository.GetAllAsync();
            var availableCars = await _carRepository.GetAvailableCarsAsync();
            var allRentals = await _rentalService.GetAllRentalsAsync();
            var activeRentals = allRentals.Where(r => r.Status == Models.RentalStatus.Active);

            ViewBag.TotalCars = allCars.Count();
            ViewBag.AvailableCars = availableCars.Count();
            ViewBag.RentedCars = allCars.Count(c => c.Status == Models.CarStatus.Rented);
            ViewBag.ActiveRentals = activeRentals.Count();
            ViewBag.CompletedRentals = allRentals.Count(r => r.Status == Models.RentalStatus.Completed);

            return View();
        }

        public async Task<IActionResult> AllRentals()
        {
            var rentals = await _rentalService.GetAllRentalsAsync();
            return View(rentals);
        }
    }
}
