namespace MyProject.Domains
{
    public class Expense
    {
        public int Id { get; set; }

        public required string Description { get; set; }

        public required decimal Amount { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public int? CarId { get; set; }
        public Car? Car { get; set; }
    }

}
