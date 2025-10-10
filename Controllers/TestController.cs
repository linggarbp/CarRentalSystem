using CarRentalSystem.Data;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    public class TestController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ICarRepository _carRepository;

        public TestController(ApplicationDbContext context, ICarRepository carRepository)
        {
            _context = context;
            _carRepository = carRepository;
        }

        public async Task<IActionResult> Database()
        {
            try
            {
                // Test direct database access
                var carsFromContext = await _context.Cars.Where(c => !c.IsDeleted).ToListAsync();
                var carsFromRepository = await _carRepository.GetAllAsync();

                var result = new
                {
                    DatabaseConnected = await _context.Database.CanConnectAsync(),
                    CarsFromContext = carsFromContext.Count,
                    CarsFromRepository = carsFromRepository.Count(),
                    Cars = carsFromContext.Select(c => new
                    {
                        c.Id,
                        c.Brand,
                        c.Model,
                        c.Status,
                        c.IsDeleted,
                        c.CreatedDate
                    }).ToList()
                };

                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        public async Task<IActionResult> CreateTestCar()
        {
            try
            {
                // Test creating car directly
                var testCar = new Car
                {
                    Brand = "TEST",
                    Model = "TestModel",
                    Year = 2023,
                    LicensePlate = "TEST-123",
                    PricePerDay = 50.00m,
                    Status = CarStatus.Available
                };

                _context.Cars.Add(testCar);
                var saveResult = await _context.SaveChangesAsync();

                return Json(new { success = true, savedChanges = saveResult, carId = testCar.Id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, error = ex.Message, stackTrace = ex.StackTrace });
            }
        }
    }
}
