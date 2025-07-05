
using Microsoft.AspNetCore.Mvc;
using MyProject.Domains.Data;
using MyProject.Web.Models;
using MyProject.Web.ViewModels;

namespace MyProject.Web.Controllers
{
    public class DashboardController : Controller
    {
        private readonly AppDbContext _context;

        public DashboardController(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var model = new DashboardViewModel
            {
                //TotalCustomers = _context.Customers.Count(),
                //TotalCars = _context.Cars.Count(),
                //AvailableCars = _context.Cars.Count(c => c.Status == CarStatus.Available),
                ////TotalDrivers = 0, // حاليًا لا يوجد كيان للسائقين Driver، لو هتضيفه عدّل هنا.
                //OngoingContracts = _context.RentalContracts.Count(rc => rc.Status == ContractStatus.Ongoing),
                //TotalRevenue = _context.Payments.Sum(p => p.Amount),
                //TotalFines = _context.Fines.Count()
            };

            return View(model);
        }
    }
}
