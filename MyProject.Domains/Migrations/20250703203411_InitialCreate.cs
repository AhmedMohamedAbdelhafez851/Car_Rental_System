using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MyProject.Domains.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cars",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Brand = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Model = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PlateNumber = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    DailyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cars", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Customers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FullName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NationalId = table.Column<string>(type: "nvarchar(14)", maxLength: 14, nullable: false),
                    DrivingLicenseNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LicenseExpiryDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LicenseImagePath = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Customers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Expenses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expenses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Expenses_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Rentals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CustomerId = table.Column<int>(type: "int", nullable: false),
                    CarId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DailyRate = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    TotalAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    AmountPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    FuelCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DelayCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    DamageCharge = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    InitialFuelLevel = table.Column<int>(type: "int", nullable: false),
                    ReturnedFuelLevel = table.Column<int>(type: "int", nullable: true),
                    InitialOdometer = table.Column<int>(type: "int", nullable: false),
                    ReturnedOdometer = table.Column<int>(type: "int", nullable: true),
                    RemainingAmount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Rentals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Rentals_Cars_CarId",
                        column: x => x.CarId,
                        principalTable: "Cars",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Rentals_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Penalties",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RentalId = table.Column<int>(type: "int", nullable: false),
                    Reason = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Penalties", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Penalties_Rentals_RentalId",
                        column: x => x.RentalId,
                        principalTable: "Rentals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cars",
                columns: new[] { "Id", "Brand", "DailyRate", "Model", "Notes", "PlateNumber", "Status" },
                values: new object[,]
                {
                    { 1, "Chevrolet", 250m, "Lanos", "عداد 170,000 كم", "س ل ج 1234", 0 },
                    { 2, "Toyota", 450m, "Corolla 2015", "تكييف ممتاز", "ط ي ر 7896", 0 }
                });

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "CreatedAt", "DrivingLicenseNumber", "FullName", "LicenseExpiryDate", "LicenseImagePath", "NationalId", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, new DateTime(2027, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL2023001", "أحمد علي", new DateTime(2027, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "29805251234567", "01012345678" },
                    { 2, new DateTime(2027, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), "DL2024005", "محمد سمير", new DateTime(2026, 10, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null, "29910101555666", "01234567890" }
                });

            migrationBuilder.InsertData(
                table: "Expenses",
                columns: new[] { "Id", "Amount", "CarId", "CreatedAt", "Description" },
                values: new object[,]
                {
                    { 1, 300m, 1, new DateTime(2025, 6, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "تغيير زيت" },
                    { 2, 200m, 2, new DateTime(2025, 6, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), "بنزين 92" }
                });

            migrationBuilder.InsertData(
                table: "Rentals",
                columns: new[] { "Id", "AmountPaid", "CarId", "CreatedAt", "CustomerId", "DailyRate", "DamageCharge", "DelayCharge", "EndDate", "FuelCharge", "InitialFuelLevel", "InitialOdometer", "RemainingAmount", "ReturnedFuelLevel", "ReturnedOdometer", "StartDate", "Status", "TotalAmount" },
                values: new object[] { 1, 250m, 1, new DateTime(2027, 5, 7, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 250m, 0m, 0m, new DateTime(2025, 6, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), 50m, 2, 170000, 0m, 1, 170300, new DateTime(2025, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 500m });

            migrationBuilder.InsertData(
                table: "Penalties",
                columns: new[] { "Id", "Amount", "CreatedAt", "Reason", "RentalId" },
                values: new object[] { 1, 100m, new DateTime(2027, 5, 6, 0, 0, 0, 0, DateTimeKind.Unspecified), "مخالفة ركن في الممنوع", 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Cars_PlateNumber",
                table: "Cars",
                column: "PlateNumber",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Customers_NationalId",
                table: "Customers",
                column: "NationalId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_CarId",
                table: "Expenses",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Penalties_RentalId",
                table: "Penalties",
                column: "RentalId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CarId",
                table: "Rentals",
                column: "CarId");

            migrationBuilder.CreateIndex(
                name: "IX_Rentals_CustomerId",
                table: "Rentals",
                column: "CustomerId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expenses");

            migrationBuilder.DropTable(
                name: "Penalties");

            migrationBuilder.DropTable(
                name: "Rentals");

            migrationBuilder.DropTable(
                name: "Cars");

            migrationBuilder.DropTable(
                name: "Customers");
        }
    }
}
