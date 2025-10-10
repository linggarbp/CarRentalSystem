using CarRentalSystem.Data;
using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace CarRentalSystem.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : BaseEntity
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _dbSet = context.Set<T>();
        }

        public virtual async Task<IEnumerable<T>> GetAllAsync()
        {
            try
            {
                return await _dbSet.Where(e => !e.IsDeleted).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetAllAsync Error: {ex.Message}");
                throw;
            }
        }

        public virtual async Task<T?> GetByIdAsync(int id)
        {
            try
            {
                return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"GetByIdAsync Error: {ex.Message}");
                throw;
            }
        }

        public virtual async Task<IEnumerable<T>> FindAsync(Expression<Func<T, bool>> predicate)
        {
            try
            {
                return await _dbSet.Where(predicate).Where(e => !e.IsDeleted).AsNoTracking().ToListAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"FindAsync Error: {ex.Message}");
                throw;
            }
        }

        public virtual async Task<T> AddAsync(T entity)
        {
            try
            {
                Console.WriteLine($"Adding entity: {entity.GetType().Name}");

                entity.CreatedDate = DateTime.Now;
                entity.IsDeleted = false;

                await _dbSet.AddAsync(entity);
                var changes = await _context.SaveChangesAsync();

                Console.WriteLine($"SaveChangesAsync returned: {changes}");
                Console.WriteLine($"Entity ID after save: {entity.Id}");

                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"AddAsync Error: {ex.Message}");
                Console.WriteLine($"Stack Trace: {ex.StackTrace}");
                throw;
            }
        }

        public virtual async Task<T> UpdateAsync(T entity)
        {
            try
            {
                Console.WriteLine($"Updating entity ID: {entity.Id}");

                _dbSet.Update(entity);
                var changes = await _context.SaveChangesAsync();

                Console.WriteLine($"Update SaveChangesAsync returned: {changes}");

                return entity;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"UpdateAsync Error: {ex.Message}");
                throw;
            }
        }

        public virtual async Task<bool> DeleteAsync(int id)
        {
            try
            {
                var entity = await GetByIdAsync(id);
                if (entity == null)
                {
                    Console.WriteLine($"Entity with ID {id} not found for deletion");
                    return false;
                }

                entity.IsDeleted = true;
                await UpdateAsync(entity);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DeleteAsync Error: {ex.Message}");
                return false;
            }
        }

        public virtual async Task<bool> ExistsAsync(int id)
        {
            try
            {
                return await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ExistsAsync Error: {ex.Message}");
                return false;
            }
        }
    }
}
