using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestReg.Migrations
{
    /// <inheritdoc />
    public partial class opus0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AnimalId",
                table: "VetAppointments",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VetAppointments_Animals_AnimalId",
                table: "VetAppointments");

            migrationBuilder.DropIndex(
                name: "IX_VetAppointments_AnimalId",
                table: "VetAppointments");

            migrationBuilder.DropColumn(
                name: "AnimalId",
                table: "VetAppointments");
        }
    }
}
