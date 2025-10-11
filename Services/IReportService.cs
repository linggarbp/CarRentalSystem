using CarRentalSystem.Models;

namespace CarRentalSystem.Services
{
    public interface IReportService
    {
        Task<byte[]> GenerateRentalsExcelReportAsync(IEnumerable<Rental> rentals);
        Task<byte[]> GenerateRentalsPdfReportAsync(IEnumerable<Rental> rentals);
    }
}
