using CarRentalSystem.Data;
using CarRentalSystem.Models;
using CarRentalSystem.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IReportService _reportService;

        public AdminController(ApplicationDbContext context, IReportService reportService)
        {
            _context = context;
            _reportService = reportService;
        }

        public async Task<IActionResult> Index()
        {
            var allCars = await _context.Cars.Where(c => !c.IsDeleted).ToListAsync();
            var availableCars = allCars.Where(c => c.Status == Models.CarStatus.Available);
            var allRentals = await _context.Rentals
                .Include(r => r.User)
                .Include(r => r.Car)
                .Where(r => !r.IsDeleted)
                .ToListAsync();
            var activeRentals = allRentals.Where(r => r.Status == Models.RentalStatus.Active);

            ViewBag.TotalCars = allCars.Count();
            ViewBag.AvailableCars = availableCars.Count();
            ViewBag.RentedCars = allCars.Count(c => c.Status == Models.CarStatus.Rented);
            ViewBag.ActiveRentals = activeRentals.Count();
            ViewBag.CompletedRentals = allRentals.Count(r => r.Status == Models.RentalStatus.Completed);
            ViewBag.TotalRevenue = allRentals.Where(r => r.Status == Models.RentalStatus.Completed).Sum(r => r.TotalPrice);

            return View();
        }

        public async Task<IActionResult> AllRentals(string sortBy = "date", string sortOrder = "desc",
    string statusFilter = "", DateTime? startDate = null, DateTime? endDate = null, string searchUser = "")
        {
            var query = _context.Rentals
                .Include(r => r.Car)
                .Include(r => r.User)
                .Where(r => !r.IsDeleted);

            // Apply filters
            if (!string.IsNullOrEmpty(statusFilter) && Enum.TryParse<RentalStatus>(statusFilter, out var status))
            {
                query = query.Where(r => r.Status == status);
            }

            if (startDate.HasValue)
            {
                query = query.Where(r => r.StartDate >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                query = query.Where(r => r.EndDate <= endDate.Value);
            }

            if (!string.IsNullOrEmpty(searchUser))
            {
                query = query.Where(r => r.User.FirstName.Contains(searchUser) ||
                                        r.User.LastName.Contains(searchUser) ||
                                        r.User.Email.Contains(searchUser));
            }

            // Apply sorting
            query = sortBy.ToLower() switch
            {
                "user" => sortOrder == "asc" ? query.OrderBy(r => r.User.FirstName) : query.OrderByDescending(r => r.User.FirstName),
                "car" => sortOrder == "asc" ? query.OrderBy(r => r.Car.Brand).ThenBy(r => r.Car.Model) : query.OrderByDescending(r => r.Car.Brand).ThenByDescending(r => r.Car.Model),
                "startdate" => sortOrder == "asc" ? query.OrderBy(r => r.StartDate) : query.OrderByDescending(r => r.StartDate),
                "enddate" => sortOrder == "asc" ? query.OrderBy(r => r.EndDate) : query.OrderByDescending(r => r.EndDate),
                "price" => sortOrder == "asc" ? query.OrderBy(r => r.TotalPrice) : query.OrderByDescending(r => r.TotalPrice),
                "status" => sortOrder == "asc" ? query.OrderBy(r => r.Status) : query.OrderByDescending(r => r.Status),
                _ => sortOrder == "asc" ? query.OrderBy(r => r.CreatedDate) : query.OrderByDescending(r => r.CreatedDate) // default: date
            };

            var rentals = await query.ToListAsync();

            // Pass filter values to view
            ViewBag.SortBy = sortBy;
            ViewBag.SortOrder = sortOrder;
            ViewBag.StatusFilter = statusFilter;
            ViewBag.StartDate = startDate?.ToString("yyyy-MM-dd");
            ViewBag.EndDate = endDate?.ToString("yyyy-MM-dd");
            ViewBag.SearchUser = searchUser;

            return View(rentals);
        }


        // EXPORT TO EXCEL
        public async Task<IActionResult> ExportRentalsToExcel()
        {
            try
            {
                var rentals = await _context.Rentals
                    .Include(r => r.Car)
                    .Include(r => r.User)
                    .Where(r => !r.IsDeleted)
                    .OrderByDescending(r => r.CreatedDate)
                    .ToListAsync();

                var excelData = await _reportService.GenerateRentalsExcelReportAsync(rentals);

                var fileName = $"Rental_Report_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

                return File(excelData,
                    "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                    fileName);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating Excel report: {ex.Message}";
                return RedirectToAction(nameof(AllRentals));
            }
        }

        public async Task<IActionResult> RentalsReportHtml(DateTime? startDate = null, DateTime? endDate = null, string? status = null)
        {
            try
            {
                var query = _context.Rentals
                    .Include(r => r.Car)
                    .Include(r => r.User)
                    .Where(r => !r.IsDeleted);

                // Apply filters
                if (startDate.HasValue)
                    query = query.Where(r => r.StartDate >= startDate.Value);

                if (endDate.HasValue)
                    query = query.Where(r => r.EndDate <= endDate.Value);

                if (!string.IsNullOrEmpty(status) && Enum.TryParse<RentalStatus>(status, out var rentalStatus))
                    query = query.Where(r => r.Status == rentalStatus);

                var rentals = await query.OrderByDescending(r => r.CreatedDate).ToListAsync();

                // SAFE ViewBag assignments
                ViewBag.GeneratedDate = DateTime.Now;
                ViewBag.HasDateFilter = startDate.HasValue || endDate.HasValue;
                ViewBag.StartDateFormatted = startDate?.ToString("MMM dd, yyyy");
                ViewBag.EndDateFormatted = endDate?.ToString("MMM dd, yyyy");
                ViewBag.StatusFilter = status;
                ViewBag.TotalRentals = rentals.Count;
                ViewBag.ActiveRentals = rentals.Count(r => r.Status == RentalStatus.Active);
                ViewBag.CompletedRentals = rentals.Count(r => r.Status == RentalStatus.Completed);
                ViewBag.TotalRevenue = rentals.Where(r => r.Status == RentalStatus.Completed).Sum(r => r.TotalPrice);

                return View(rentals);
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating report: {ex.Message}";
                return RedirectToAction(nameof(AllRentals));
            }
        }


        // UPDATE ExportRentalsToPdf method to redirect to HTML
        public async Task<IActionResult> ExportRentalsToPdf()
        {
            // Redirect to HTML report instead
            return RedirectToAction("RentalsReportHtml");
        }

        // UPDATE ExportFilteredRentals for PDF format
        public async Task<IActionResult> ExportFilteredRentals(DateTime? startDate, DateTime? endDate, string? status, string format = "excel")
        {
            try
            {
                if (format.ToLower() == "pdf")
                {
                    // Redirect to HTML report with filters
                    return RedirectToAction("RentalsReportHtml", new { startDate, endDate, status });
                }
                else
                {
                    // Keep Excel export as is
                    var query = _context.Rentals
                        .Include(r => r.Car)
                        .Include(r => r.User)
                        .Where(r => !r.IsDeleted);

                    if (startDate.HasValue)
                        query = query.Where(r => r.StartDate >= startDate.Value);

                    if (endDate.HasValue)
                        query = query.Where(r => r.EndDate <= endDate.Value);

                    if (!string.IsNullOrEmpty(status) && Enum.TryParse<RentalStatus>(status, out var rentalStatus))
                        query = query.Where(r => r.Status == rentalStatus);

                    var rentals = await query.OrderByDescending(r => r.CreatedDate).ToListAsync();
                    var excelData = await _reportService.GenerateRentalsExcelReportAsync(rentals);

                    var dateFilter = "";
                    if (startDate.HasValue || endDate.HasValue)
                    {
                        dateFilter = $"_{startDate?.ToString("yyyyMMdd") ?? "start"}_{endDate?.ToString("yyyyMMdd") ?? "end"}";
                    }

                    var fileName = $"Rental_Report{dateFilter}_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";
                    return File(excelData,
                        "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                        fileName);
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error generating report: {ex.Message}";
                return RedirectToAction(nameof(AllRentals));
            }
        }

    }
}
