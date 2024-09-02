using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestReg.Migrations
{
    /// <inheritdoc />
    public partial class VetAdmin5 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VetAppointments_Animals_AnimalId",
                table: "VetAppointments");

            migrationBuilder.DropIndex(
                name: "IX_VetAppointments_AnimalId",
                table: "VetAppointments");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_VetAppointments_AnimalId",
                table: "VetAppointments",
                column: "AnimalId");

            migrationBuilder.AddForeignKey(
                name: "FK_VetAppointments_Animals_AnimalId",
                table: "VetAppointments",
                column: "AnimalId",
                principalTable: "Animals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
