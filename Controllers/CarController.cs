using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CarController> _logger;

        public CarController(ApplicationDbContext context, ILogger<CarController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // GET: Car
        public async Task<IActionResult> Index(string sort)
        {
            var query = _context.Cars.Where(c => !c.IsDeleted);

            query = sort switch
            {
                "oldest" => query.OrderBy(c => c.CreatedDate),
                "brand" => query.OrderBy(c => c.Brand).ThenBy(c => c.Model),
                "newest" or _ => query.OrderByDescending(c => c.CreatedDate)
            };

            var cars = await query.ToListAsync();
            return View(cars);
        }


        // GET: Car/Details/1
        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // GET: Car/Create
        public IActionResult Create()
        {
            return View(new CarViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarViewModel model)
        {
            try
            {
                Console.WriteLine("=== HYBRID CREATE DEBUG ===");
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

                // If model binding failed, try manual extraction
                if (model == null ||
                    (string.IsNullOrEmpty(model.Brand) && string.IsNullOrEmpty(model.Model)))
                {
                    Console.WriteLine("Model binding failed, trying manual extraction...");

                    model = new CarViewModel
                    {
                        Brand = Request.Form["Brand"].ToString(),
                        Model = Request.Form["Model"].ToString(),
                        LicensePlate = Request.Form["LicensePlate"].ToString(),
                        ImageUrl = Request.Form["ImageUrl"].ToString()
                    };

                    // Parse numbers manually
                    if (int.TryParse(Request.Form["Year"], out int year))
                        model.Year = year;

                    if (decimal.TryParse(Request.Form["PricePerDay"], out decimal price))
                        model.PricePerDay = price;

                    Console.WriteLine($"Manual extraction - Brand: '{model.Brand}', Model: '{model.Model}'");
                }

                // Manual validation
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(model.Brand))
                    errors.Add("Brand is required");

                if (string.IsNullOrWhiteSpace(model.Model))
                    errors.Add("Model is required");

                if (string.IsNullOrWhiteSpace(model.LicensePlate))
                    errors.Add("License plate is required");

                if (model.Year < 1900 || model.Year > 2030)
                    errors.Add("Year must be between 1900 and 2030");

                if (model.PricePerDay <= 0)
                    errors.Add("Price per day must be greater than 0");

                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }

                var car = new Car
                {
                    Brand = model.Brand.Trim(),
                    Model = model.Model.Trim(),
                    Year = model.Year,
                    LicensePlate = model.LicensePlate.Trim(),
                    PricePerDay = model.PricePerDay,
                    ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim(),
                    Status = CarStatus.Available
                };

                Console.WriteLine($"Creating car: {car.Brand} {car.Model}");

                _context.Cars.Add(car);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Car created with ID: {car.Id}");

                TempData["Success"] = $"Car {car.Brand} {car.Model} created successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Create Exception: {ex.Message}");
                TempData["Error"] = $"Error creating car: {ex.Message}";
                return View(model ?? new CarViewModel());
            }
        }


        // GET: Car/Edit/1
        public async Task<IActionResult> Edit(int id)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (car == null)
            {
                return NotFound();
            }

            var model = new CarViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                Year = car.Year,
                LicensePlate = car.LicensePlate,
                PricePerDay = car.PricePerDay,
                ImageUrl = car.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarViewModel model)
        {
            try
            {
                Console.WriteLine($"=== HYBRID EDIT DEBUG: ID = {id} ===");
                Console.WriteLine($"Model.Id: {model?.Id ?? 0}");

                // If model binding failed, try manual extraction
                if (model == null || model.Id == 0 ||
                    (string.IsNullOrEmpty(model.Brand) && string.IsNullOrEmpty(model.Model)))
                {
                    Console.WriteLine("Model binding failed for edit, trying manual extraction...");

                    model = new CarViewModel
                    {
                        Id = id, // Use route parameter
                        Brand = Request.Form["Brand"].ToString(),
                        Model = Request.Form["Model"].ToString(),
                        LicensePlate = Request.Form["LicensePlate"].ToString(),
                        ImageUrl = Request.Form["ImageUrl"].ToString()
                    };

                    // Parse numbers manually
                    if (int.TryParse(Request.Form["Year"], out int year))
                        model.Year = year;

                    if (int.TryParse(Request.Form["PricePerDay"], out int price))
                        model.PricePerDay = price;
                }

                if (id != model.Id)
                {
                    Console.WriteLine($"ID mismatch: route={id}, model={model.Id}");
                    model.Id = id; // Fix ID mismatch
                }

                // Manual validation
                var errors = new List<string>();

                if (string.IsNullOrWhiteSpace(model.Brand))
                    errors.Add("Brand is required");

                if (string.IsNullOrWhiteSpace(model.Model))
                    errors.Add("Model is required");

                if (string.IsNullOrWhiteSpace(model.LicensePlate))
                    errors.Add("License plate is required");

                if (model.Year < 1900 || model.Year > 2030)
                    errors.Add("Year must be between 1900 and 2030");

                if (model.PricePerDay <= 0)
                    errors.Add("Price per day must be greater than 0");

                if (errors.Any())
                {
                    foreach (var error in errors)
                    {
                        ModelState.AddModelError("", error);
                    }
                    return View(model);
                }

                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
                if (car == null)
                {
                    return NotFound();
                }

                Console.WriteLine($"Updating car: {model.Brand} {model.Model}");

                car.Brand = model.Brand.Trim();
                car.Model = model.Model.Trim();
                car.Year = model.Year;
                car.LicensePlate = model.LicensePlate.Trim();
                car.PricePerDay = model.PricePerDay;
                car.ImageUrl = string.IsNullOrWhiteSpace(model.ImageUrl) ? null : model.ImageUrl.Trim();

                _context.Cars.Update(car);
                await _context.SaveChangesAsync();

                Console.WriteLine($"Car updated successfully");

                TempData["Success"] = $"Car {car.Brand} {car.Model} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit Exception: {ex.Message}");
                TempData["Error"] = $"Error updating car: {ex.Message}";
                return View(model ?? new CarViewModel { Id = id });
            }
        }


        // GET: Car/Delete/1
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars
                .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

            if (car == null)
            {
                return NotFound();
            }

            return View(car);
        }

        // POST: Car/Delete/1
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var car = await _context.Cars
                    .FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

                if (car != null)
                {
                    car.IsDeleted = true; // Soft delete
                    _context.Cars.Update(car);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = $"Car {car.Brand} {car.Model} deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Car not found.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error deleting car with ID {CarId}", id);
                TempData["Error"] = "An error occurred while deleting the car.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
