using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestReg.Migrations
{
    /// <inheritdoc />
    public partial class isBooked : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsBooked",
                table: "VetAppointments",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsBooked",
                table: "VetAppointments");
        }
    }
}
