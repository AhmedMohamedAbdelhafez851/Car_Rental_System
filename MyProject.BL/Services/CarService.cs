// MyProject.BL/Services/CarService.cs
using Microsoft.EntityFrameworkCore; // Ensure this is included for ToListAsync
using Microsoft.Extensions.Caching.Memory;
using MyProject.BL.Abstraction;
using MyProject.BL.UnitOfWork;
using MyProject.Domains;
using MyProject.Domains.Enums;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProject.BL.Services
{
    public class CarService : ICarService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMemoryCache _cache;
        private const string AvailableCarsCacheKey = "AvailableCars";

        public CarService(IUnitOfWork unitOfWork, IMemoryCache cache)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _cache = cache ?? throw new ArgumentNullException(nameof(cache));
        }

        public async Task<List<Car>> GetAvailableCarsAsync()
        {
            if (!_cache.TryGetValue(AvailableCarsCacheKey, out List<Car>? cars)) // Allow nullable to handle CS8600
            {
                var queryTask = _unitOfWork.Repository<Car>().GetAllIncludingAsync(); // Use GetAllIncludingAsync
                var query = await queryTask;
                cars = await query
                    .Where(c => c.Status == CarStatus.متاحه)
                    .AsNoTracking() // Now applies to IQueryable<Car>
                    .ToListAsync();
                _cache.Set(AvailableCarsCacheKey, cars, TimeSpan.FromMinutes(5));
            }
            return cars ?? new List<Car>(); // Handle null case for CS8603
        }

        public async Task<Car> GetCarByIdAsync(int id)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(id);
            if (car == null)
                throw new InvalidOperationException("السيارة غير موجودة");
            return car;
        }

        public async Task UpdateCarStatusAsync(int carId, CarStatus status)
        {
            var car = await GetCarByIdAsync(carId);
            car.Status = status;
            await _unitOfWork.Repository<Car>().UpdateAsync(car);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}