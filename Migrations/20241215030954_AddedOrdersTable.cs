using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISSTechLogistics.Migrations
{
    /// <inheritdoc />
    public partial class AddedOrdersTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    ShipmentId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Origin = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(6,3)", precision: 6, scale: 3, nullable: false),
                    DeliveryTime = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.ShipmentId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Orders");
        }
    }
}
