
using MyProject.Domains;

namespace MyProject.Web.ViewModels
{
    public class CustomerPageViewModel
    {
        public Customer NewCustomer { get; set; } = new Customer();
        public List<Customer> Customers { get; set; } = new List<Customer>();
    }
}
