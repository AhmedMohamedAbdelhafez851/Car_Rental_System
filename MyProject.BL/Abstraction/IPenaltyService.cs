// MyProject.BL/Abstraction/IPenaltyService.cs
using MyProject.Domains;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProject.BL.Abstraction
{
    public interface IPenaltyService
    {
        Task AddPenaltiesAsync(int rentalId, List<Penalty> penalties);
        Task<List<Penalty>> GetPenaltiesByRentalIdAsync(int rentalId);
    }
}