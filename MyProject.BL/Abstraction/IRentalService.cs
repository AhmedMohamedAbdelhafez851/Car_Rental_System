using MyProject.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.BL.Abstraction
{
    // BL/Services/IRentalService.cs
    public interface IRentalService
    {
        Task<List<Rental>> GetRentalsAsync(string searchQuery);
        Task<Rental> GetRentalDetailsAsync(int id);
        Task<(List<Customer>, List<Car>)> PrepareQuickRentalDataAsync();
        Task<bool> CreateQuickRentalAsync(Rental rental);
        Task<Rental> PrepareCheckInDataAsync(int id);
        Task<bool> CheckInAsync(Rental rental, decimal? additionalPayment, List<Penalty> penalties, bool needsMaintenance);
    }
}
