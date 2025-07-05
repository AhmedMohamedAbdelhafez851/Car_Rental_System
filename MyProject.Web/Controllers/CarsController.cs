// CarsController.cs
using Microsoft.AspNetCore.Mvc;

using Microsoft.EntityFrameworkCore;
using MyProject.Domains;
using MyProject.Domains.Data;
using MyProject.Web.ViewModels;

namespace MyProject.Web.Controllers
{
    public class CarsController : Controller
    {
        private readonly AppDbContext _context;

        public CarsController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var cars = await _context.Cars
                .Select(c => new CarViewModel
                {
                    Id = c.Id,
                    Brand = c.Brand,
                    Model = c.Model,
                    PlateNumber = c.PlateNumber,
                    DailyRate = c.DailyRate,
                    Status = c.Status,
                    Notes = c.Notes
                })
                .ToListAsync();
            return View(cars);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateOrUpdate(CarViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Car car;
                if (viewModel.Id == 0) // إضافة سيارة جديدة
                {
                    car = new Car
                    {
                        Brand = viewModel.Brand,
                        Model = viewModel.Model,
                        PlateNumber = viewModel.PlateNumber,
                        DailyRate = viewModel.DailyRate,
                        Status = viewModel.Status,
                        Notes = viewModel.Notes
                    };
                    _context.Add(car);
                }
                else // تعديل سيارة موجودة
                {
                    car = await _context.Cars.FindAsync(viewModel.Id);
                    if (car == null)
                    {
                        return NotFound();
                    }
                    car.Brand = viewModel.Brand;
                    car.Model = viewModel.Model;
                    car.PlateNumber = viewModel.PlateNumber;
                    car.DailyRate = viewModel.DailyRate;
                    car.Status = viewModel.Status;
                    car.Notes = viewModel.Notes;
                    _context.Update(car);
                }

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = viewModel.Id == 0 ? "تم إضافة السيارة بنجاح" : "تم تعديل السيارة بنجاح" });
            }

            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
            return Json(new { success = false, errors = errors });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return Json(new { success = false, message = "السيارة غير موجودة" });
            }

            _context.Cars.Remove(car);
            await _context.SaveChangesAsync();
            return Json(new { success = true, message = "تم حذف السيارة بنجاح" });
        }

        public async Task<IActionResult> GetCar(int id)
        {
            var car = await _context.Cars.FindAsync(id);
            if (car == null)
            {
                return NotFound();
            }

            var viewModel = new CarViewModel
            {
                Id = car.Id,
                Brand = car.Brand,
                Model = car.Model,
                PlateNumber = car.PlateNumber,
                DailyRate = car.DailyRate,
                Status = car.Status,
                Notes = car.Notes
            };

            return Json(viewModel);
        }
    }
}