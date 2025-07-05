// MyProject.Web/ViewModels/CheckInViewModel.cs
using MyProject.Domains.Enums;
using System.ComponentModel.DataAnnotations;

namespace MyProject.Web.ViewModels
{
    public class CheckInViewModel
    {
        public int RentalId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CarDetails { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public FuelLevel InitialFuelLevel { get; set; }
        public int? InitialOdometer { get; set; }
        [Required(ErrorMessage = "تاريخ الرجوع مطلوب")]
        public DateTime ReturnDate { get; set; }
        [Required(ErrorMessage = "مستوى البنزين عند الرجوع مطلوب")]
        public FuelLevel ReturnedFuelLevel { get; set; } // Non-nullable with [Required]
        [Required(ErrorMessage = "قراءة العداد عند الرجوع مطلوبة")]
        public int ReturnedOdometer { get; set; } // Changed to non-nullable int
        public decimal? DamageCharge { get; set; }
        public decimal? AdditionalPayment { get; set; }
        public bool NeedsMaintenance { get; set; }
        public List<PenaltyViewModel> Penalties { get; set; } = new List<PenaltyViewModel>();
    }
}