using MyProject.Domains.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Domains
{

    public class Car
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الماركة مطلوبة")]
        [Display(Name = "الماركة")]
        public string Brand { get; set; } = string.Empty;

        [Required(ErrorMessage = "الموديل مطلوب")]
        [Display(Name = "الموديل")]
        public string Model { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم اللوحة مطلوب")]
        [Display(Name = "رقم اللوحة")]
        public string PlateNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "السعر اليومي مطلوب")]
        [Display(Name = "السعر اليومي")]
        [Range(100, 10000, ErrorMessage = "السعر يجب أن يكون بين 100 و 10,000 جنيه")]
        public decimal DailyRate { get; set; }

        [Display(Name = "الحالة")]
        public CarStatus Status { get; set; } = CarStatus.متاحه;

        [Display(Name = "ملاحظات")]
        public string? Notes { get; set; }
    }
}