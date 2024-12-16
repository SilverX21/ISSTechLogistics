using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ISSTechLogistics.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedOrderFilesTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsLatest",
                table: "OrderFiles",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsLatest",
                table: "OrderFiles");
        }
    }
}
