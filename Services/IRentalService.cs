using CarRentalSystem.Models;
using CarRentalSystem.ViewModels;

namespace CarRentalSystem.Services
{
    public interface IRentalService
    {
        Task<bool> RentCarAsync(RentCarViewModel model, string userId);
        Task<bool> ReturnCarAsync(int rentalId);
        Task<decimal> CalculateTotalPriceAsync(int carId, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Rental>> GetUserRentalsAsync(string userId);
        Task<IEnumerable<Rental>> GetAllRentalsAsync();
        Task<bool> CanUserRentCarAsync(int carId, string userId);
    }
}
