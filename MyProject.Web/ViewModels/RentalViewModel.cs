

using MyProject.Domains.Enums;

namespace MyProject.Web.ViewModels
{
    public class RentalViewModel
    {
        public int Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string CarDetails { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal AmountPaid { get; set; }
        public RentalStatus Status { get; set; }


    }
}