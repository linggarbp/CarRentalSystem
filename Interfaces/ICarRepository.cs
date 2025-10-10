using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;

namespace CarRentalSystem.Interfaces
{
    public interface ICarRepository : IGenericRepository<Car>
    {
        Task<IEnumerable<Car>> GetAvailableCarsAsync();
        Task<bool> UpdateCarStatusAsync(int carId, CarStatus status);
    }
}
