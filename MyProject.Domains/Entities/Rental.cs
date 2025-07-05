using System.ComponentModel.DataAnnotations;
using MyProject.Domains.Enums; 
namespace MyProject.Domains
{

    public class Rental
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "العميل مطلوب")]
        [Display(Name = "العميل")]
        public int CustomerId { get; set; }
        public Customer Customer { get; set; } = null!;

        [Required(ErrorMessage = "السيارة مطلوبة")]
        [Display(Name = "السيارة")]
        public int CarId { get; set; }
        public Car Car { get; set; } = null!;

        [Display(Name = "تاريخ الحجز")]
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "تاريخ الاستلام مطلوب")]
        [Display(Name = "تاريخ الاستلام")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "تاريخ التسليم مطلوب")]
        [Display(Name = "تاريخ التسليم")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "السعر اليومي مطلوب")]
        [Display(Name = "السعر اليومي")]
        public decimal DailyRate { get; set; }

        [Required(ErrorMessage = "المبلغ الإجمالي مطلوب")]
        [Display(Name = "الإجمالي")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "المبلغ المدفوع مطلوب")]
        [Display(Name = "المدفوع")]
        public decimal AmountPaid { get; set; }

        [Display(Name = "رسوم الوقود")]
        public decimal FuelCharge { get; set; }

        [Display(Name = "رسوم التأخير")]
        public decimal DelayCharge { get; set; }

        [Display(Name = "رسوم التلفيات")]
        public decimal DamageCharge { get; set; }

        [Display(Name = "المبلغ النهائي")]
        public decimal FinalAmount => TotalAmount + FuelCharge + DelayCharge + DamageCharge;

        [Required(ErrorMessage = "مستوى الوقود مطلوب")]
        [Display(Name = "الوقود عند الاستلام")]
        public FuelLevel InitialFuelLevel { get; set; }

        [Display(Name = "الوقود عند التسليم")]
        public FuelLevel? ReturnedFuelLevel { get; set; }

        [Required(ErrorMessage = "قراءة العداد مطلوبة")]
        [Display(Name = "العداد عند الاستلام")]
        public int? InitialOdometer { get; set; }

        [Display(Name = "العداد عند التسليم")]
        public int? ReturnedOdometer { get; set; }

        [Display(Name = "المبلغ المتبقي")]
        public decimal RemainingAmount { get; set; }

        [Display(Name = "الحالة")]
        public RentalStatus Status { get; set; } = RentalStatus.نشط;

        public ICollection<Penalty> Penalties { get; set; } = new List<Penalty>();
    }
}