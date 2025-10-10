using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace YourProjectName.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CarController(ApplicationDbContext context)
        {
            _context = context;
        }

        // ADD THIS METHOD TO DEBUG DATABASE
        public async Task<IActionResult> TestDatabase(int id = 1)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

                if (car == null)
                {
                    return Json(new { success = false, message = $"Car with ID {id} not found" });
                }

                return Json(new
                {
                    success = true,
                    carData = new
                    {
                        car.Id,
                        car.Brand,
                        car.Model,
                        car.Year,
                        car.LicensePlate,
                        car.PricePerDay,
                        car.ImageUrl,
                        car.Status,
                        car.IsDeleted,
                        car.CreatedDate
                    }
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message });
            }
        }

        // GET: Car
        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars.Where(c => !c.IsDeleted).ToListAsync();
            return View(cars);
        }

        // GET: Car/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (car == null)
            {
                TempData["Error"] = "Car not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // GET: Car/Create
        public IActionResult Create()
        {
            // Initialize empty model to prevent null reference
            var model = new CarViewModel();
            return View(model);
        }


        // POST: Car/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CarViewModel model)
        {
            try
            {
                Console.WriteLine("=== CREATE CAR STARTED ===");
                Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

                // DEBUG: Print all incoming values
                Console.WriteLine($"Brand: '{model.Brand}'");
                Console.WriteLine($"Model: '{model.Model}'");
                Console.WriteLine($"Year: {model.Year}");
                Console.WriteLine($"LicensePlate: '{model.LicensePlate}'");
                Console.WriteLine($"PricePerDay: {model.PricePerDay}");
                Console.WriteLine($"ImageUrl: '{model.ImageUrl}'");
                
                if (!ModelState.IsValid)
                {
                    Console.WriteLine("=== MODEL VALIDATION ERRORS ===");
                    foreach (var kvp in ModelState)
                    {
                        Console.WriteLine($"Field: {kvp.Key}");
                        foreach (var error in kvp.Value.Errors)
                        {
                            Console.WriteLine($"  Error: {error.ErrorMessage}");
                        }
                    }

                    // Set ViewBag for debugging
                    ViewBag.ValidationErrors = ModelState
                        .Where(x => x.Value.Errors.Count > 0)
                        .ToDictionary(
                            kvp => kvp.Key,
                            kvp => kvp.Value.Errors.Select(e => e.ErrorMessage).ToArray()
                        );

                    return View(model);
                }

                if (ModelState.IsValid)
                {
                    var car = new Car
                    {
                        Brand = model.Brand,
                        Model = model.Model,
                        Year = model.Year,
                        LicensePlate = model.LicensePlate,
                        PricePerDay = model.PricePerDay,
                        ImageUrl = model.ImageUrl,
                        Status = CarStatus.Available,
                        CreatedDate = DateTime.Now,
                        IsDeleted = false
                    };

                    Console.WriteLine($"Adding car: {car.Brand} {car.Model}");

                    _context.Cars.Add(car);
                    var changes = await _context.SaveChangesAsync();

                    Console.WriteLine($"SaveChanges returned: {changes}");
                    Console.WriteLine($"Car ID: {car.Id}");

                    if (changes > 0)
                    {
                        TempData["Success"] = $"Car {car.Brand} {car.Model} created successfully!";
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        TempData["Error"] = "No changes were saved to database.";
                    }
                }
                else
                {
                    Console.WriteLine("=== MODEL VALIDATION ERRORS ===");
                    foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                    {
                        Console.WriteLine($"Error: {error.ErrorMessage}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"CREATE EXCEPTION: {ex.Message}");
                TempData["Error"] = $"Error: {ex.Message}";
                return View(model);
            }

            return View(model);
        }

        // GET: Car/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                Console.WriteLine($"=== EDIT GET: ID = {id} ===");

                // Test direct database query
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);

                if (car == null)
                {
                    Console.WriteLine($"Car with ID {id} not found");
                    TempData["Error"] = $"Car with ID {id} not found.";
                    return RedirectToAction(nameof(Index));
                }

                // Log all car properties
                Console.WriteLine($"=== CAR FROM DATABASE ===");
                Console.WriteLine($"ID: {car.Id}");
                Console.WriteLine($"Brand: '{car.Brand}'");
                Console.WriteLine($"Model: '{car.Model}'");
                Console.WriteLine($"Year: {car.Year}");
                Console.WriteLine($"LicensePlate: '{car.LicensePlate}'");
                Console.WriteLine($"PricePerDay: {car.PricePerDay}");
                Console.WriteLine($"ImageUrl: '{car.ImageUrl}'");
                Console.WriteLine($"Status: {car.Status}");
                Console.WriteLine($"IsDeleted: {car.IsDeleted}");

                // Create model step by step
                var model = new CarViewModel();
                model.Id = car.Id;
                model.Brand = car.Brand ?? "";
                model.Model = car.Model ?? "";
                model.Year = car.Year;
                model.LicensePlate = car.LicensePlate ?? "";
                model.PricePerDay = car.PricePerDay;
                model.ImageUrl = car.ImageUrl;

                // Log model properties
                Console.WriteLine($"=== VIEW MODEL CREATED ===");
                Console.WriteLine($"Model.ID: {model.Id}");
                Console.WriteLine($"Model.Brand: '{model.Brand}'");
                Console.WriteLine($"Model.Model: '{model.Model}'");
                Console.WriteLine($"Model.Year: {model.Year}");
                Console.WriteLine($"Model.LicensePlate: '{model.LicensePlate}'");
                Console.WriteLine($"Model.PricePerDay: {model.PricePerDay}");
                Console.WriteLine($"Model.ImageUrl: '{model.ImageUrl}'");

                return View(model);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit GET Exception: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                TempData["Error"] = $"Error loading car: {ex.Message}";
                return RedirectToAction(nameof(Index));
            }
        }


        // POST: Car/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CarViewModel model)
        {
            try
            {
                Console.WriteLine($"=== EDIT POST DEBUG ===");
                Console.WriteLine($"Route ID: {id}");
                Console.WriteLine($"Model.Id: {model.Id}");
                Console.WriteLine($"Brand: '{model.Brand}'");
                Console.WriteLine($"Model: '{model.Model}'");
                Console.WriteLine($"Year: {model.Year}");
                Console.WriteLine($"LicensePlate: '{model.LicensePlate}'");
                Console.WriteLine($"PricePerDay: {model.PricePerDay}");

                // Fix: If model.Id is 0, use route id
                if (model.Id == 0)
                {
                    Console.WriteLine($"Model.Id was 0, setting to route id: {id}");
                    model.Id = id;
                }

                if (id != model.Id)
                {
                    Console.WriteLine($"ID mismatch: route={id}, model={model.Id}");
                    TempData["Error"] = $"Invalid car ID. Route: {id}, Model: {model.Id}";
                    return RedirectToAction(nameof(Index));
                }

                if (!ModelState.IsValid)
                {
                    Console.WriteLine("=== EDIT MODEL VALIDATION ERRORS ===");
                    foreach (var kvp in ModelState)
                    {
                        Console.WriteLine($"Field: {kvp.Key}");
                        foreach (var error in kvp.Value.Errors)
                        {
                            Console.WriteLine($"  Error: {error.ErrorMessage}");
                        }
                    }
                    return View(model);
                }

                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
                if (car == null)
                {
                    Console.WriteLine($"Car with ID {id} not found in database");
                    TempData["Error"] = "Car not found.";
                    return RedirectToAction(nameof(Index));
                }

                Console.WriteLine($"Found car in DB: {car.Brand} {car.Model}");

                // Update car properties
                car.Brand = model.Brand;
                car.Model = model.Model;
                car.Year = model.Year;
                car.LicensePlate = model.LicensePlate;
                car.PricePerDay = model.PricePerDay;
                car.ImageUrl = model.ImageUrl;

                _context.Cars.Update(car);
                var changes = await _context.SaveChangesAsync();

                Console.WriteLine($"Update SaveChanges returned: {changes}");

                TempData["Success"] = $"Car {car.Brand} {car.Model} updated successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Edit POST Exception: {ex.Message}");
                Console.WriteLine($"Stack: {ex.StackTrace}");
                TempData["Error"] = $"Error updating car: {ex.Message}";
                return View(model);
            }
        }


        // GET: Car/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
            if (car == null)
            {
                TempData["Error"] = "Car not found.";
                return RedirectToAction(nameof(Index));
            }
            return View(car);
        }

        // POST: Car/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var car = await _context.Cars.FirstOrDefaultAsync(c => c.Id == id && !c.IsDeleted);
                if (car != null)
                {
                    car.IsDeleted = true;
                    _context.Cars.Update(car);
                    await _context.SaveChangesAsync();

                    TempData["Success"] = "Car deleted successfully!";
                }
                else
                {
                    TempData["Error"] = "Car not found.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error deleting car: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
