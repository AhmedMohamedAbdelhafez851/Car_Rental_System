// MyProject.BL/Abstraction/IPaymentService.cs
using MyProject.Domains.Enums;
using System;

namespace MyProject.BL.Abstraction
{
    public interface IPaymentService
    {
        decimal CalculateTotalAmount(DateTime startDate, DateTime endDate, decimal dailyRate);
        decimal CalculateFuelCharge(FuelLevel initialFuelLevel, FuelLevel? returnedFuelLevel); // Changed to nullable
        decimal CalculateDelayCharge(DateTime endDate, DateTime returnDate, decimal dailyRate);
    }
}