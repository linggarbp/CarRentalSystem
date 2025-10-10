using CarRentalSystem.Interfaces;
using CarRentalSystem.Models;
using CarRentalSystem.Services;
using CarRentalSystem.ViewModels;

namespace CarRentalSystem.Services
{
    public class RentalService : IRentalService
    {
        private readonly IRentalRepository _rentalRepository;
        private readonly ICarRepository _carRepository;

        public RentalService(IRentalRepository rentalRepository, ICarRepository carRepository)
        {
            _rentalRepository = rentalRepository;
            _carRepository = carRepository;
        }

        public async Task<bool> RentCarAsync(RentCarViewModel model, string userId)
        {
            var car = await _carRepository.GetByIdAsync(model.CarId);
            if (car == null || car.Status != CarStatus.Available)
                return false;

            var totalDays = (model.EndDate - model.StartDate).Days;
            if (totalDays <= 0) return false;

            var totalPrice = await CalculateTotalPriceAsync(model.CarId, model.StartDate, model.EndDate);

            var rental = new Rental
            {
                UserId = userId,
                CarId = model.CarId,
                StartDate = model.StartDate,
                EndDate = model.EndDate,
                TotalDays = totalDays,
                TotalPrice = totalPrice,
                Status = RentalStatus.Active
            };

            await _rentalRepository.AddAsync(rental);
            await _carRepository.UpdateCarStatusAsync(model.CarId, CarStatus.Rented);

            return true;
        }

        public async Task<bool> ReturnCarAsync(int rentalId)
        {
            var rental = await _rentalRepository.GetByIdAsync(rentalId);
            if (rental == null || rental.Status != RentalStatus.Active)
                return false;

            rental.Status = RentalStatus.Completed;
            await _rentalRepository.UpdateAsync(rental);
            await _carRepository.UpdateCarStatusAsync(rental.CarId, CarStatus.Available);

            return true;
        }

        public async Task<decimal> CalculateTotalPriceAsync(int carId, DateTime startDate, DateTime endDate)
        {
            var car = await _carRepository.GetByIdAsync(carId);
            if (car == null) return 0;

            var totalDays = (endDate - startDate).Days;
            return car.PricePerDay * totalDays;
        }

        public async Task<IEnumerable<Rental>> GetUserRentalsAsync(string userId)
        {
            return await _rentalRepository.GetRentalsByUserIdAsync(userId);
        }

        public async Task<IEnumerable<Rental>> GetAllRentalsAsync()
        {
            return await _rentalRepository.GetAllRentalsWithDetailsAsync();
        }

        public async Task<bool> CanUserRentCarAsync(int carId, string userId)
        {
            var car = await _carRepository.GetByIdAsync(carId);
            return car != null && car.Status == CarStatus.Available;
        }
    }
}
