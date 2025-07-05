// MyProject.BL/Abstraction/ICarService.cs
using MyProject.Domains;
using MyProject.Domains.Enums;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProject.BL.Abstraction
{
    public interface ICarService
    {
        Task<List<Car>> GetAvailableCarsAsync();
        Task<Car> GetCarByIdAsync(int id);
        Task UpdateCarStatusAsync(int carId, CarStatus status);
    }
}