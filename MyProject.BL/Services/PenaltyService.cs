// MyProject.BL/Services/PenaltyService.cs
using Microsoft.EntityFrameworkCore; // Required for ToListAsync
using MyProject.BL.Abstraction;
using MyProject.BL.UnitOfWork;
using MyProject.Domains;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyProject.BL.Services
{
    public class PenaltyService : IPenaltyService
    {
        private readonly IUnitOfWork _unitOfWork;

        public PenaltyService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task AddPenaltiesAsync(int rentalId, List<Penalty> penalties)
        {
            foreach (var penalty in penalties)
            {
                if (!string.IsNullOrEmpty(penalty.Reason) && penalty.Amount > 0)
                {
                    penalty.RentalId = rentalId;
                    penalty.CreatedAt = DateTime.UtcNow;
                    await _unitOfWork.Repository<Penalty>().AddAsync(penalty);
                }
            }
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task<List<Penalty>> GetPenaltiesByRentalIdAsync(int rentalId)
        {
            var queryTask = _unitOfWork.Repository<Penalty>().GetAllIncludingAsync(); // Use GetAllIncludingAsync
            var query = await queryTask;
            return await query
                .Where(p => p.RentalId == rentalId)
                .AsNoTracking() // Now applies to IQueryable<Penalty>
                .ToListAsync();
        }
    }
}