// CarViewModel.cs
using MyProject.Domains.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Web.ViewModels
{
    public class CarViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الماركة مطلوبة")]
        [Display(Name = "ماركة السيارة")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "الموديل مطلوب")]
        [Display(Name = "موديل السيارة")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم اللوحة مطلوب")]
        [Display(Name = "رقم اللوحة")]
        public string PlateNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "السعر اليومي مطلوب")]
        [Display(Name = "السعر اليومي")]
        [Range(0.01, double.MaxValue, ErrorMessage = "السعر اليومي لازم يكون أكبر من صفر")]
        public decimal DailyRate { get; set; }

        [Display(Name = "حالة السيارة")]
        public CarStatus Status { get; set; }

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}