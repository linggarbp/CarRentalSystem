using CarRentalSystem.Data;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CarRentalSystem.Repositories
{
    public class RentalRepository : GenericRepository<Rental>, IRentalRepository
    {
        public RentalRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Rental>> GetRentalsByUserIdAsync(string userId)
        {
            return await _dbSet
                .Include(r => r.Car)
                .Include(r => r.User)
                .Where(r => r.UserId == userId)
                .OrderByDescending(r => r.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetActiveRentalsAsync()
        {
            return await _dbSet
                .Include(r => r.Car)
                .Include(r => r.User)
                .Where(r => r.Status == RentalStatus.Active)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IEnumerable<Rental>> GetAllRentalsWithDetailsAsync()
        {
            return await _dbSet
                .Include(r => r.Car)
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
