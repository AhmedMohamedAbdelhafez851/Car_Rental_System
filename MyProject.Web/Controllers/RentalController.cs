// MyProject.Web/Controllers/RentalController.cs
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MyProject.BL.Abstraction;
using MyProject.Domains;
using MyProject.Domains.Enums;
using MyProject.Web.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyProject.Web.Controllers
{
    public class RentalController : Controller
    {
        private readonly IRentalService _rentalService;
        private readonly ICustomerService _customerService;
        private readonly ICarService _carService;
        private readonly ILogger<RentalController> _logger;

        public RentalController(
            IRentalService rentalService,
            ICustomerService customerService,
            ICarService carService,
            ILogger<RentalController> logger)
        {
            _rentalService = rentalService ?? throw new ArgumentNullException(nameof(rentalService));
            _customerService = customerService ?? throw new ArgumentNullException(nameof(customerService));
            _carService = carService ?? throw new ArgumentNullException(nameof(carService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        // Helper methods for manual mapping to ViewModels
        private List<RentalViewModel> ToRentalViewModels(List<Rental> rentals)
        {
            return rentals.Select(r => new RentalViewModel
            {
                Id = r.Id,
                CustomerName = r.Customer.FullName,
                CarDetails = $"{r.Car.Brand} {r.Car.Model} ({r.Car.PlateNumber})",
                StartDate = r.StartDate,
                EndDate = r.EndDate,
                TotalAmount = r.TotalAmount,
                AmountPaid = r.AmountPaid,
                Status = r.Status
            }).ToList();
        }

        private RentalDetailsViewModel ToRentalDetailsViewModel(Rental rental)
        {
            return new RentalDetailsViewModel
            {
                Id = rental.Id,
                CustomerName = rental.Customer.FullName,
                CustomerPhone = rental.Customer.PhoneNumber,
                CarDetails = $"{rental.Car.Brand} {rental.Car.Model} ({rental.Car.PlateNumber})",
                DailyRate = rental.DailyRate,
                StartDate = rental.StartDate,
                EndDate = rental.EndDate,
                InitialFuelLevel = rental.InitialFuelLevel,
                InitialOdometer = rental.InitialOdometer,
                ReturnedFuelLevel = rental.ReturnedFuelLevel,
                ReturnedOdometer = rental.ReturnedOdometer,
                TotalAmount = rental.TotalAmount,
                AmountPaid = rental.AmountPaid,
                FuelCharge = rental.FuelCharge,
                DelayCharge = rental.DelayCharge,
                DamageCharge = rental.DamageCharge,
                Status = rental.Status,
                Penalties = rental.Penalties?.Select(p => new PenaltyViewModel
                {
                    Reason = p.Reason,
                    Amount = p.Amount
                }).ToList() ?? new List<PenaltyViewModel>()
            };
        }

        private CheckInViewModel ToCheckInViewModel(Rental rental)
        {
            return new CheckInViewModel
            {
                RentalId = rental.Id,
                CustomerName = rental.Customer.FullName,
                CarDetails = $"{rental.Car?.Brand} {rental.Car?.Model} ({rental.Car?.PlateNumber})",
                StartDate = rental.StartDate,
                EndDate = rental.EndDate,
                TotalAmount = rental.TotalAmount,
                AmountPaid = rental.AmountPaid,
                DailyRate = rental.DailyRate,
                InitialFuelLevel = rental.InitialFuelLevel,
                InitialOdometer = rental.InitialOdometer,
                ReturnDate = DateTime.Now
            };
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchQuery)
        {
            try
            {
                var rentals = await _rentalService.GetRentalsAsync(searchQuery);
                var rentalViewModels = ToRentalViewModels(rentals);
                return View(rentalViewModels);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحميل قائمة الحجوزات");
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل قائمة الحجوزات.";
                return View(new List<RentalViewModel>());
            }
        }

        [HttpGet]
        public async Task<IActionResult> Quick()
        {
            try
            {
                var (customers, cars) = await _rentalService.PrepareQuickRentalDataAsync();
                var model = new CreateRentalFullViewModel
                {
                    Customers = customers.Select(c => new SelectListItem
                    {
                        Value = c.Id.ToString(),
                        Text = $"{c.FullName} ({c.PhoneNumber})"
                    }).ToList(),
                    AvailableCarsDetails = cars.Select(c => new CarDetailsVM
                    {
                        Id = c.Id,
                        DisplayText = $"{c.Brand} {c.Model}",
                        DailyRate = c.DailyRate,
                        Notes = c.Notes ?? string.Empty,
                        PlateNumber = c.PlateNumber
                    }).ToList(),
                    StartDate = DateTime.Now,
                    EndDate = DateTime.Now.AddDays(1)
                };
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحميل صفحة الحجز السريع");
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل صفحة الحجز.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Quick(CreateRentalFullViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var (customers, cars) = await _rentalService.PrepareQuickRentalDataAsync();
                model.Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.FullName} ({c.PhoneNumber})"
                }).ToList();
                model.AvailableCarsDetails = cars.Select(c => new CarDetailsVM
                {
                    Id = c.Id,
                    DisplayText = $"{c.Brand} {c.Model}",
                    DailyRate = c.DailyRate,
                    Notes = c.Notes ?? string.Empty,
                    PlateNumber = c.PlateNumber
                }).ToList();
                TempData["ErrorMessage"] = "يرجى إكمال جميع الحقول";
                return View(model);
            }

            try
            {
                var rental = new Rental
                {
                    CustomerId = model.CustomerId!.Value,
                    CarId = model.CarId,
                    StartDate = model.StartDate,
                    EndDate = model.EndDate,
                    InitialFuelLevel = model.InitialFuelLevel,
                    InitialOdometer = model.InitialOdometer,
                    AmountPaid = model.PaidAmount ?? 0
                };
                await _rentalService.CreateQuickRentalAsync(rental);
                TempData["SuccessMessage"] = "تم حفظ الحجز بنجاح";
                return RedirectToAction(nameof(Index));
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                var (customers, cars) = await _rentalService.PrepareQuickRentalDataAsync();
                model.Customers = customers.Select(c => new SelectListItem
                {
                    Value = c.Id.ToString(),
                    Text = $"{c.FullName} ({c.PhoneNumber})"
                }).ToList();
                model.AvailableCarsDetails = cars.Select(c => new CarDetailsVM
                {
                    Id = c.Id,
                    DisplayText = $"{c.Brand} {c.Model}",
                    DailyRate = c.DailyRate,
                    Notes = c.Notes ?? string.Empty,
                    PlateNumber = c.PlateNumber
                }).ToList();
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في معالجة الحجز السريع");
                TempData["ErrorMessage"] = "حدث خطأ أثناء معالجة الحجز.";
                return RedirectToAction(nameof(Quick));
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] CreateRentalFullViewModel model)
        {
            try
            {
                var customer = new Customer
                {
                    FullName = model.NewCustomerName!,
                    PhoneNumber = model.NewCustomerPhone!,
                    NationalId = model.NewCustomerNationalId!,
                    DrivingLicenseNumber = model.NewCustomerLicense!,
                    LicenseExpiryDate = model.NewCustomerLicenseExpiry,
                    CreatedAt = DateTime.UtcNow
                };
                var result = await _customerService.AddCustomerAsync(customer);
                return Json(new
                {
                    success = true,
                    customerId = result.Id,
                    text = $"{result.FullName} ({result.PhoneNumber})",
                    hasPreviousRental = result.Rentals.Any(r => r.Status != RentalStatus.ملغي)
                });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إضافة العميل");
                return Json(new { success = false, message = "حدث خطأ أثناء إضافة العميل" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetCustomers(string search)
        {
            try
            {
                var customers = await _customerService.GetCustomersAsync(search);
                var result = customers.Select(c => new
                {
                    id = c.Id,
                    fullName = c.FullName,
                    phoneNumber = c.PhoneNumber,
                    hasPreviousRental = c.Rentals.Any(r => r.Status != RentalStatus.ملغي)
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب العملاء");
                return Json(new { success = false, message = "حدث خطأ أثناء جلب العملاء" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAvailableCars()
        {
            try
            {
                var cars = await _carService.GetAvailableCarsAsync();
                var result = cars.Select(c => new
                {
                    id = c.Id,
                    plateNumber = c.PlateNumber,
                    brand = c.Brand,
                    model = c.Model,
                    dailyRate = c.DailyRate
                }).ToList();
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في جلب السيارات");
                return Json(new { success = false, message = "حدث خطأ أثناء جلب السيارات" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            try
            {
                var rental = await _rentalService.GetRentalDetailsAsync(id);
                var viewModel = ToRentalDetailsViewModel(rental);
                return View(viewModel);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحميل تفاصيل الحجز");
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل تفاصيل الحجز.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckIn(int id)
        {
            try
            {
                var rental = await _rentalService.PrepareCheckInDataAsync(id);
                var viewModel = ToCheckInViewModel(rental);
                return View(viewModel);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في تحميل صفحة إنهاء الحجز");
                TempData["ErrorMessage"] = "حدث خطأ أثناء تحميل صفحة إنهاء الحجز.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckIn(CheckInViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return Json(new { success = false, message = "البيانات غير صالحة: " + string.Join(", ", errors) });
            }

            try
            {
                var rental = new Rental
                {
                    Id = model.RentalId,
                    ReturnedOdometer = model.ReturnedOdometer, // Now int
                    ReturnedFuelLevel = model.ReturnedFuelLevel, // Non-nullable, validated by ModelState
                    DamageCharge = model.DamageCharge ?? 0
                };
                var penalties = model.Penalties?.Select(p => new Penalty
                {
                    Reason = p.Reason,
                    Amount = p.Amount
                }).ToList() ?? new List<Penalty>();

                await _rentalService.CheckInAsync(rental, model.AdditionalPayment, penalties, model.NeedsMaintenance);
                return Json(new { success = true, message = "تم إنهاء الحجز بنجاح" });
            }
            catch (InvalidOperationException ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطأ في إنهاء الحجز");
                return Json(new { success = false, message = "حدث خطأ أثناء إنهاء الحجز" });
            }
        }
    }
}