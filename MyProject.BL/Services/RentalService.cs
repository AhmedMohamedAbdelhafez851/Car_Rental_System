// MyProject.BL/Services/RentalService.cs
using Microsoft.EntityFrameworkCore;
using MyProject.BL.Abstraction;
using MyProject.BL.UnitOfWork;
using MyProject.Domains;
using MyProject.Domains.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.BL.Services
{
    public class RentalService : IRentalService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;
        //private readonly string AvailableCarsCacheKey = "AvailableCars";

        public RentalService(IUnitOfWork unitOfWork, IPaymentService paymentService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _paymentService = paymentService ?? throw new ArgumentNullException(nameof(paymentService));
        }

        public async Task<List<Rental>> GetRentalsAsync(string searchQuery)
        {
            var queryTask = _unitOfWork.Repository<Rental>().GetAllIncludingAsync(r => r.Customer, r => r.Car);
            var query = await queryTask;

            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(r => r.Id.ToString().Contains(searchQuery) ||
                                         r.Customer.FullName.Contains(searchQuery) ||
                                         r.Car.PlateNumber.Contains(searchQuery));
            }

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Rental> GetRentalDetailsAsync(int id)
        {
            var queryTask = _unitOfWork.Repository<Rental>().GetAllIncludingAsync(r => r.Customer, r => r.Car, r => r.Penalties);
            var query = await queryTask;

            var rental = await query.AsNoTracking()
                .FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
                throw new InvalidOperationException("الحجز غير موجود");

            return rental;
        }

        public async Task<(List<Customer>, List<Car>)> PrepareQuickRentalDataAsync()
        {
            var customersQueryTask = _unitOfWork.Repository<Customer>().GetAllIncludingAsync();
            var customersQuery = await customersQueryTask;
            var customers = await customersQuery.AsNoTracking().ToListAsync();

            var carsQueryTask = _unitOfWork.Repository<Car>().GetAllIncludingAsync();
            var carsQuery = await carsQueryTask;
            var cars = await carsQuery
                .Where(c => c.Status == CarStatus.متاحه)
                .AsNoTracking()
                .ToListAsync();

            return (customers, cars);
        }

        public async Task<bool> CreateQuickRentalAsync(Rental rental)
        {
            var car = await _unitOfWork.Repository<Car>().GetByIdAsync(rental.CarId);
            if (car == null || car.Status != CarStatus.متاحه)
                throw new InvalidOperationException("السيارة غير متاحة");

            var activeRentalForCustomer = await _unitOfWork.Repository<Rental>()
                .GetList(r => r.CustomerId == rental.CustomerId && r.Status == RentalStatus.نشط);
            if (activeRentalForCustomer.Any())
                throw new InvalidOperationException("العميل لديه حجز نشط");

            var activeRentalForCar = await _unitOfWork.Repository<Rental>()
                .GetList(r => r.CarId == rental.CarId && r.Status == RentalStatus.نشط);
            if (activeRentalForCar.Any())
                throw new InvalidOperationException("السيارة محجوزة");

            if (rental.EndDate <= rental.StartDate.AddMinutes(1))
                throw new InvalidOperationException("تاريخ التسليم يجب أن يكون بعد الاستلام");

            rental.DailyRate = car.DailyRate;
            rental.TotalAmount = _paymentService.CalculateTotalAmount(rental.StartDate, rental.EndDate, car.DailyRate);
            rental.Status = RentalStatus.نشط;
            rental.CreatedAt = DateTime.UtcNow;

            using var transaction = await _unitOfWork.BeginTransactionAsync();
            try
            {
                await _unitOfWork.Repository<Rental>().AddAsync(rental);
                car.Status = CarStatus.مؤجره;
                await _unitOfWork.Repository<Car>().UpdateAsync(car);
                await _unitOfWork.SaveChangesAsync();
                _unitOfWork.Commit();
                return true;
            }
            catch
            {
                _unitOfWork.Rollback();
                throw;
            }
        }

        public async Task<List<Penalty>> GetPenaltiesByRentalIdAsync(int rentalId)
        {
            var queryTask = _unitOfWork.Repository<Penalty>().GetAllIncludingAsync(); // Use GetAllIncludingAsync
            var query = await queryTask;
            return await query
                .Where(p => p.RentalId == rentalId)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Rental> PrepareCheckInDataAsync(int id)
        {
            var queryTask = _unitOfWork.Repository<Rental>().GetAllIncludingAsync(r => r.Customer, r => r.Car);
            var query = await queryTask;

            var rental = await query.FirstOrDefaultAsync(r => r.Id == id);

            if (rental == null)
                throw new InvalidOperationException("الحجز غير موجود");

            if (rental.Status != RentalStatus.نشط)
                throw new InvalidOperationException("لا يمكن إنهاء الحجز لأنه ليس نشطًا");

            return rental;
        }

        public async Task<bool> CheckInAsync(Rental rental, decimal? additionalPayment, List<Penalty> penalties, bool needsMaintenance)
        {
            var queryTask = _unitOfWork.Repository<Rental>().GetAllIncludingAsync(r => r.Car);
            var query = await queryTask;

            var existingRental = await query.FirstOrDefaultAsync(r => r.Id == rental.Id);

            if (existingRental == null)
                throw new InvalidOperationException("الحجز غير موجود");

            if (existingRental.Status != RentalStatus.نشط)
                throw new InvalidOperationException("لا يمكن إنهاء الحجز لأنه ليس نشطًا");

            if (rental.ReturnedOdometer < existingRental.InitialOdometer)
                throw new InvalidOperationException("قراءة العداد عند الرجوع غير صالحة");

            existingRental.ReturnedOdometer = rental.ReturnedOdometer;
            existingRental.ReturnedFuelLevel = rental.ReturnedFuelLevel;
            existingRental.FuelCharge = _paymentService.CalculateFuelCharge(existingRental.InitialFuelLevel, rental.ReturnedFuelLevel);
            existingRental.DelayCharge = _paymentService.CalculateDelayCharge(existingRental.EndDate, rental.CreatedAt, existingRental.DailyRate);
            existingRental.DamageCharge = rental.DamageCharge;
            existingRental.AmountPaid += additionalPayment ?? 0;
            existingRental.Status = RentalStatus.تم_الإرجاع;

            if (penalties != null)
            {
                foreach (var penalty in penalties)
                {
                    if (!string.IsNullOrEmpty(penalty.Reason) && penalty.Amount > 0)
                    {
                        penalty.RentalId = existingRental.Id;
                        penalty.CreatedAt = DateTime.UtcNow;
                        await _unitOfWork.Repository<Penalty>().AddAsync(penalty);
                    }
                }
            }

            existingRental.Car.Status = needsMaintenance ? CarStatus.تحت_الصيانه : CarStatus.متاحه;

            await _unitOfWork.Repository<Rental>().UpdateAsync(existingRental);
            await _unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<List<Car>> GetAvailableCarsAsync()
        {
            var queryTask = _unitOfWork.Repository<Car>().GetAllIncludingAsync();
            var query = await queryTask;
            var cars = await query
                .Where(c => c.Status == CarStatus.متاحه)
                .AsNoTracking()
                .ToListAsync();
            //_cache.Set(AvailableCarsCacheKey, cars, TimeSpan.FromMinutes(5));
            return cars;
        }
    }
}