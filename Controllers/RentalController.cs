using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Services;
using CarRentalSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CarRentalSystem.Controllers
{
    [Authorize(Roles = "User")]
    public class RentalController : Controller
    {
        private readonly IRentalService _rentalService;
        private readonly ICarRepository _carRepository;
        private readonly UserManager<ApplicationUser> _userManager;

        public RentalController(
            IRentalService rentalService,
            ICarRepository carRepository,
            UserManager<ApplicationUser> userManager)
        {
            _rentalService = rentalService;
            _carRepository = carRepository;
            _userManager = userManager;
        }

        // GET: My Rentals
        public async Task<IActionResult> MyRentals()
        {
            var userId = _userManager.GetUserId(User);
            var rentals = await _rentalService.GetUserRentalsAsync(userId!);
            return View(rentals);
        }

        // GET: Rent Car
        public async Task<IActionResult> RentCar(int carId)
        {
            var car = await _carRepository.GetByIdAsync(carId);
            if (car == null || car.Status != CarStatus.Available)
            {
                TempData["Error"] = "Car is not available for rent.";
                return RedirectToAction("Index", "Home");
            }

            var model = new RentCarViewModel
            {
                CarId = carId,
                CarInfo = $"{car.Brand} {car.Model} ({car.Year})",
                PricePerDay = car.PricePerDay,
                StartDate = DateTime.Today.AddDays(1),
                EndDate = DateTime.Today.AddDays(2)
            };

            return View(model);
        }

        // POST: Rent Car
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RentCar(RentCarViewModel model)
        {
            if (model.StartDate <= DateTime.Today)
            {
                ModelState.AddModelError("StartDate", "Start date must be in the future");
            }

            if (model.EndDate <= model.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date must be after start date");
            }

            if (ModelState.IsValid)
            {
                var userId = _userManager.GetUserId(User);
                var success = await _rentalService.RentCarAsync(model, userId!);

                if (success)
                {
                    TempData["Success"] = "Car rented successfully!";
                    return RedirectToAction(nameof(MyRentals));
                }

                ModelState.AddModelError(string.Empty, "Failed to rent car. Car may no longer be available.");
            }

            // Reload car info if validation fails
            var car = await _carRepository.GetByIdAsync(model.CarId);
            if (car != null)
            {
                model.CarInfo = $"{car.Brand} {car.Model} ({car.Year})";
                model.PricePerDay = car.PricePerDay;
            }

            return View(model);
        }

        // POST: Return Car
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnCar(int rentalId)
        {
            var success = await _rentalService.ReturnCarAsync(rentalId);

            if (success)
            {
                TempData["Success"] = "Car returned successfully!";
            }
            else
            {
                TempData["Error"] = "Failed to return car.";
            }

            return RedirectToAction(nameof(MyRentals));
        }

        // AJAX: Calculate Price
        [HttpPost]
        public async Task<IActionResult> CalculatePrice(int carId, DateTime startDate, DateTime endDate)
        {
            if (endDate <= startDate)
            {
                return Json(new { success = false, message = "End date must be after start date" });
            }

            var totalPrice = await _rentalService.CalculateTotalPriceAsync(carId, startDate, endDate);
            var totalDays = (endDate - startDate).Days;

            return Json(new { success = true, totalPrice, totalDays });
        }
    }
}
