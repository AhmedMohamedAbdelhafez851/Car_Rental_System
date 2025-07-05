
using MyProject.Domains.Enums;

namespace MyProject.Web.ViewModels
{
    public class RentalDetailsViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CustomerPhone { get; set; } = string.Empty;
        public string CarDetails { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal DailyRate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public decimal FuelCharge { get; set; }
        public decimal DelayCharge { get; set; }
        public decimal DamageCharge { get; set; }
        public FuelLevel InitialFuelLevel { get; set; }
        public FuelLevel? ReturnedFuelLevel { get; set; }
        public int? InitialOdometer { get; set; }
        public int? ReturnedOdometer { get; set; }
        public RentalStatus Status { get; set; }
        public List<PenaltyViewModel> Penalties { get; set; } = new List<PenaltyViewModel>();
    }
}