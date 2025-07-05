// MyProject.BL/Services/CustomerService.cs
using Microsoft.EntityFrameworkCore;
using MyProject.BL.Abstraction;
using MyProject.BL.UnitOfWork;
using MyProject.Domains;

namespace MyProject.BL.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly IUnitOfWork _unitOfWork;

        public CustomerService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            if (string.IsNullOrWhiteSpace(customer.FullName))
                throw new ArgumentException("الاسم الكامل مطلوب");
            if (string.IsNullOrWhiteSpace(customer.PhoneNumber))
                throw new ArgumentException("رقم الهاتف مطلوب");
            if (string.IsNullOrWhiteSpace(customer.NationalId))
                throw new ArgumentException("الرقم القومي مطلوب");
            if (string.IsNullOrWhiteSpace(customer.DrivingLicenseNumber))
                throw new ArgumentException("رقم الرخصة مطلوب");
            if (!customer.LicenseExpiryDate.HasValue)
                throw new ArgumentException("تاريخ انتهاء الرخصة مطلوب");

            // Use AnyAsync directly on the repository
            if (await _unitOfWork.Repository<Customer>().AnyAsync(c => c.FullName == customer.FullName))
                throw new InvalidOperationException("الاسم موجود مسبقًا");
            if (await _unitOfWork.Repository<Customer>().AnyAsync(c => c.NationalId == customer.NationalId))
                throw new InvalidOperationException("الرقم القومي موجود مسبقًا");
            if (await _unitOfWork.Repository<Customer>().AnyAsync(c => c.PhoneNumber == customer.PhoneNumber))
                throw new InvalidOperationException("رقم الهاتف موجود مسبقًا");

            customer.CreatedAt = DateTime.UtcNow;
            await _unitOfWork.Repository<Customer>().AddAsync(customer);
            await _unitOfWork.SaveChangesAsync();
            return customer;
        }

        public async Task<List<Customer>> GetCustomersAsync(string searchQuery)
        {
            var query = await _unitOfWork.Repository<Customer>().GetAllIncludingAsync(c => c.Rentals);
            if (!string.IsNullOrEmpty(searchQuery))
            {
                query = query.Where(c => c.FullName.Contains(searchQuery) || c.PhoneNumber.Contains(searchQuery));
            }
            return await query.AsNoTracking().Take(10).ToListAsync();
        }
    }
}