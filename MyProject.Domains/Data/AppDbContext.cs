using Microsoft.EntityFrameworkCore;
using MyProject.Domains.Enums;
namespace MyProject.Domains.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Customer> Customers => Set<Customer>();
        public DbSet<Car> Cars => Set<Car>();
        public DbSet<Rental> Rentals => Set<Rental>();
        public DbSet<Penalty> Penalties => Set<Penalty>();
        public DbSet<Expense> Expenses => Set<Expense>();

        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // إضافة فهارس
            modelBuilder.Entity<Customer>()
                .HasIndex(c => c.NationalId)
                .IsUnique();
            modelBuilder.Entity<Car>()
                .HasIndex(c => c.PlateNumber)
                .IsUnique();
            modelBuilder.Entity<Rental>()
                .HasIndex(r => r.CustomerId);
            modelBuilder.Entity<Rental>()
                .HasIndex(r => r.CarId);

            // بيانات التهيئة
            modelBuilder.Entity<Car>().HasData(
                new Car
                {
                    Id = 1,
                    Brand = "Chevrolet",
                    Model = "Lanos",
                    PlateNumber = "س ل ج 1234",
                    DailyRate = 250,
                    Status = CarStatus.متاحه,
                    Notes = "عداد 170,000 كم"
                },
                new Car
                {
                    Id = 2,
                    Brand = "Toyota",
                    Model = "Corolla 2015",
                    PlateNumber = "ط ي ر 7896",
                    DailyRate = 450,
                    Status = CarStatus.متاحه,
                    Notes = "تكييف ممتاز"
                }
            );

            modelBuilder.Entity<Customer>().HasData(
                new Customer
                {
                    Id = 1,
                    FullName = "أحمد علي",
                    PhoneNumber = "01012345678",
                    NationalId = "29805251234567",
                    DrivingLicenseNumber = "DL2023001",
                    LicenseExpiryDate = new DateTime(2027, 5, 1),
                    CreatedAt = new DateTime(2027, 5, 1)
                },
                new Customer
                {
                    Id = 2,
                    FullName = "محمد سمير",
                    PhoneNumber = "01234567890",
                    NationalId = "29910101555666",
                    DrivingLicenseNumber = "DL2024005",
                    LicenseExpiryDate = new DateTime(2026, 10, 1),
                    CreatedAt = new DateTime(2027, 5, 2)
                }
            );

            modelBuilder.Entity<Rental>().HasData(
                new Rental
                {
                    Id = 1,
                    CustomerId = 1,
                    CarId = 1,
                    StartDate = new DateTime(2025, 6, 20),
                    EndDate = new DateTime(2025, 6, 22),
                    DailyRate = 250,
                    TotalAmount = 500,
                    AmountPaid = 250,
                    FuelCharge = 50,
                    DelayCharge = 0,
                    DamageCharge = 0,
                    InitialFuelLevel = FuelLevel.نص,
                    ReturnedFuelLevel = FuelLevel.ربع,
                    InitialOdometer = 170000,
                    ReturnedOdometer = 170300,
                    Status = RentalStatus.تم_الإرجاع,
                    CreatedAt = new DateTime(2027, 5, 7)
                }
            );

            modelBuilder.Entity<Penalty>().HasData(
                new Penalty
                {
                    Id = 1,
                    RentalId = 1,
                    Reason = "مخالفة ركن في الممنوع",
                    Amount = 100,
                    CreatedAt = new DateTime(2027, 5, 6)
                }
            );

            modelBuilder.Entity<Expense>().HasData(
                new Expense
                {
                    Id = 1,
                    Description = "تغيير زيت",
                    Amount = 300,
                    CreatedAt = new DateTime(2025, 6, 15),
                    CarId = 1
                },
                new Expense
                {
                    Id = 2,
                    Description = "بنزين 92",
                    Amount = 200,
                    CreatedAt = new DateTime(2025, 6, 16),
                    CarId = 2
                }
            );
        }
    }
}