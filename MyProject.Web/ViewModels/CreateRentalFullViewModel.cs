using Microsoft.AspNetCore.Mvc.Rendering;
using MyProject.Domains.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Web.ViewModels
{
    public class CreateRentalFullViewModel
    {
        public int? CustomerId { get; set; }
        public List<SelectListItem> Customers { get; set; } = new List<SelectListItem>();

        [Display(Name = "الاسم الكامل")]
        public string? NewCustomerName { get; set; }
        [Display(Name = "رقم الهاتف")]
        public string? NewCustomerPhone { get; set; }
        [Display(Name = "الرقم القومي")]
        public string? NewCustomerNationalId { get; set; }
        [Display(Name = "رقم رخصة القيادة")]
        public string? NewCustomerLicense { get; set; }
        [Display(Name = "تاريخ انتهاء الرخصة")]
        public DateTime? NewCustomerLicenseExpiry { get; set; }

        [Required(ErrorMessage = "يجب اختيار سيارة")]
        [Display(Name = "السيارة")]
        public int CarId { get; set; }
        public List<CarDetailsVM> AvailableCarsDetails { get; set; } = new List<CarDetailsVM>();

        [Required(ErrorMessage = "تاريخ الاستلام مطلوب")]
        [Display(Name = "تاريخ الاستلام")]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "تاريخ التسليم مطلوب")]
        [Display(Name = "تاريخ التسليم")]
        public DateTime EndDate { get; set; } = DateTime.Now.AddDays(1);

        [Required(ErrorMessage = "مستوى البنزين مطلوب")]
        [Display(Name = "مستوى البنزين عند التسليم")]
        public FuelLevel InitialFuelLevel { get; set; }

        [Required(ErrorMessage = "قراءة العداد مطلوبة")]
        [Display(Name = "قراءة العداد")]
        public int? InitialOdometer { get; set; }

        [Display(Name = "المبلغ المدفوع")]
        public decimal? PaidAmount { get; set; }
    }
}