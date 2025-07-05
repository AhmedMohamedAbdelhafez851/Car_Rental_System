namespace MyProject.Web.ViewModels
{
    public class CarDetailsVM
    {
        public int Id { get; set; }
        public string DisplayText { get; set; } = string.Empty;
        public decimal DailyRate { get; set; }
        public string Notes { get; set; } = string.Empty;
        public string PlateNumber { get; set; } // Add this property
    }
}
