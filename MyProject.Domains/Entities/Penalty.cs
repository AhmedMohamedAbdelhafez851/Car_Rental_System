// ✅ الغرامات (Fines)


namespace MyProject.Domains
{
    public class Penalty
    {
        public int Id { get; set; }

        public int RentalId { get; set; }
        public Rental Rental { get; set; } = null!;

        public required string Reason { get; set; }
        public required decimal Amount { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }

}