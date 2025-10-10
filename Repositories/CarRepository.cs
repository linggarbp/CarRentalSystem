using CarRentalSystem.Data;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Repositories
{
    public class CarRepository : GenericRepository<Car>, ICarRepository
    {
        public CarRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Car>> GetAvailableCarsAsync()
        {
            return await _dbSet
                .Where(c => c.Status == CarStatus.Available)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> UpdateCarStatusAsync(int carId, CarStatus status)
        {
            var car = await GetByIdAsync(carId);
            if (car == null) return false;

            car.Status = status;
            await UpdateAsync(car);
            return true;
        }
    }
}
