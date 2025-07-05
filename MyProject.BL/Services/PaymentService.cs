// MyProject.BL/Services/PaymentService.cs
using MyProject.BL.Abstraction;
using MyProject.Domains.Enums;
using System;

namespace MyProject.BL.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly decimal _delayPenaltyRate = 1.5m; // 50% زيادة على السعر اليومي كرسوم تأخير
        private readonly decimal _fuelChargePerLevel = 250m; // 250 جنيه لكل مستوى نقص في الوقود

        public decimal CalculateTotalAmount(DateTime startDate, DateTime endDate, decimal dailyRate)
        {
            if (endDate <= startDate)
                throw new ArgumentException("تاريخ التسليم يجب أن يكون بعد تاريخ الاستلام");

            var days = (decimal)(endDate - startDate).TotalDays;
            return Math.Round(days * dailyRate, 0);
        }

        public decimal CalculateFuelCharge(FuelLevel initialFuelLevel, FuelLevel? returnedFuelLevel)
        {
            if (!returnedFuelLevel.HasValue)
                return 0; // Or throw an exception, depending on your requirements

            if (returnedFuelLevel.Value >= initialFuelLevel)
                return 0;

            var fuelLevels = new[] { FuelLevel.فاضي, FuelLevel.ربع, FuelLevel.نص, FuelLevel.تلات_تربع, FuelLevel.مليان };
            var initialIndex = Array.IndexOf(fuelLevels, initialFuelLevel);
            var returnedIndex = Array.IndexOf(fuelLevels, returnedFuelLevel.Value);

            return (initialIndex - returnedIndex) * _fuelChargePerLevel;
        }

        public decimal CalculateDelayCharge(DateTime endDate, DateTime returnDate, decimal dailyRate)
        {
            if (returnDate <= endDate)
                return 0;

            var extraDays = (decimal)(returnDate - endDate).TotalDays;
            return Math.Round(extraDays * dailyRate * _delayPenaltyRate, 0);
        }
    }
}