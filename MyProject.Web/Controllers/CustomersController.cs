using Microsoft.AspNetCore.Mvc;
using MyProject.Domains;
using MyProject.Domains.Data;
namespace MyProject.Web.Controllers
{
    public class CustomersController : Controller
    {
        private readonly AppDbContext _context;

        public CustomersController(AppDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (ModelState.IsValid)
            {
                customer.CreatedAt = DateTime.Now;
                _context.Customers.Add(customer);
                await _context.SaveChangesAsync();

                return Json(new
                {
                    success = true,
                    customer = new
                    {
                        id = customer.Id,
                        text = $"{customer.FullName} - {customer.PhoneNumber}"
                    }
                });
            }

            var errors = ModelState.Values
                .SelectMany(v => v.Errors)
                .Select(e => e.ErrorMessage)
                .ToList();

            return Json(new { success = false, message = "خطأ في البيانات", errors });
        }

        // GET: Customers

    }
}
