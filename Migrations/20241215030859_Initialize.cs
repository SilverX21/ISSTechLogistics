using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISSTechLogistics.Migrations
{
    /// <inheritdoc />
    public partial class Initialize : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.CreateTable(
                name: "OrdersDetailsStatistics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TotalOrders = table.Column<int>(type: "int", nullable: false),
                    AverageWeight = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    AverageDeliveryTime = table.Column<decimal>(type: "decimal(4,2)", precision: 4, scale: 2, nullable: false),
                    CalculationDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsLatest = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrdersDetailsStatistics", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrdersDetailsStatistics");

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    ShipmentID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DeliveryTime = table.Column<int>(type: "int", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Origin = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ShipmentID);
                });
        }
    }
}
