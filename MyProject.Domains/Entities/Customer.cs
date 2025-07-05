using System.ComponentModel.DataAnnotations;

namespace MyProject.Domains
{
    public class Customer
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم الكامل مطلوب")]
        [Display(Name = "الاسم الكامل")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم الهاتف مطلوب")]
        [Display(Name = "رقم الهاتف")]
        [Phone(ErrorMessage = "رقم هاتف غير صحيح")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "الرقم القومي مطلوب")]
        [Display(Name = "الرقم القومي")]
        [StringLength(14, MinimumLength = 14, ErrorMessage = "الرقم القومي يجب أن يكون 14 رقمًا")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "رقم رخصة القيادة مطلوب")]
        [Display(Name = "رقم الرخصة")]
        public string DrivingLicenseNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "تاريخ انتهاء الرخصة مطلوب")]
        [Display(Name = "انتهاء الرخصة")]
        [DataType(DataType.Date)]
        public DateTime? LicenseExpiryDate { get; set; }

        [Display(Name = "صورة الرخصة")]
        public string? LicenseImagePath { get; set; }

        [Display(Name = "تاريخ التسجيل")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Rental> Rentals { get; set; } = new List<Rental>();
    }
}