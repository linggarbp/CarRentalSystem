using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;

namespace CarRentalSystem.Interfaces
{
    public interface IRentalRepository : IGenericRepository<Rental>
    {
        Task<IEnumerable<Rental>> GetRentalsByUserIdAsync(string userId);
        Task<IEnumerable<Rental>> GetActiveRentalsAsync();
        Task<IEnumerable<Rental>> GetAllRentalsWithDetailsAsync();
    }
}
