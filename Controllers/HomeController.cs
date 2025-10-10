using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using CarRentalSystem.Data;

namespace CarRentalSystem.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICarRepository _carRepository;

        public HomeController(ICarRepository carRepository)
        {
            _carRepository = carRepository;
        }

        public async Task<IActionResult> Index()
        {
            var availableCars = await _carRepository.GetAvailableCarsAsync();
            return View(availableCars);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
