using MyProject.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyProject.BL.Abstraction
{
    public interface ICustomerService
    {
        Task<Customer> AddCustomerAsync(Customer customer);
        Task<List<Customer>> GetCustomersAsync(string searchQuery);
    }
}
