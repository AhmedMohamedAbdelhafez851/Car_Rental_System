namespace MyProject.Web.ViewModels
{
    public class DashboardViewModel
    {
        public int TotalCustomers { get; set; }
        public int TotalCars { get; set; }
        public int AvailableCars { get; set; }
        public int TotalDrivers { get; set; }
        public int OngoingContracts { get; set; }
        public int TotalFines { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
