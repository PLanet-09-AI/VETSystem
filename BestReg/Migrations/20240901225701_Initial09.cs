using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestReg.Migrations
{
    /// <inheritdoc />
    public partial class Initial09 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HealthMetrics",
                table: "MedicalRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "CheckupDate",
                table: "MedicalRecords",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "HealthMetricsId",
                table: "MedicalRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IllnessTreatmentInfoId",
                table: "MedicalRecords",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Status",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "HealthMetrics",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Weight = table.Column<double>(type: "float", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    HeartRate = table.Column<double>(type: "float", nullable: false),
                    RespirationRate = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HealthMetrics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IllnessTreatmentInfo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Diagnosis = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Treatment = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsCritical = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IllnessTreatmentInfo", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_HealthMetricsId",
                table: "MedicalRecords",
                column: "HealthMetricsId");

            migrationBuilder.CreateIndex(
                name: "IX_MedicalRecords_IllnessTreatmentInfoId",
                table: "MedicalRecords",
                column: "IllnessTreatmentInfoId");

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_HealthMetrics_HealthMetricsId",
                table: "MedicalRecords",
                column: "HealthMetricsId",
                principalTable: "HealthMetrics",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MedicalRecords_IllnessTreatmentInfo_IllnessTreatmentInfoId",
                table: "MedicalRecords",
                column: "IllnessTreatmentInfoId",
                principalTable: "IllnessTreatmentInfo",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_HealthMetrics_HealthMetricsId",
                table: "MedicalRecords");

            migrationBuilder.DropForeignKey(
                name: "FK_MedicalRecords_IllnessTreatmentInfo_IllnessTreatmentInfoId",
                table: "MedicalRecords");

            migrationBuilder.DropTable(
                name: "HealthMetrics");

            migrationBuilder.DropTable(
                name: "IllnessTreatmentInfo");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_HealthMetricsId",
                table: "MedicalRecords");

            migrationBuilder.DropIndex(
                name: "IX_MedicalRecords_IllnessTreatmentInfoId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "CheckupDate",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "HealthMetricsId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "IllnessTreatmentInfoId",
                table: "MedicalRecords");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Animals");

            migrationBuilder.AddColumn<string>(
                name: "HealthMetrics",
                table: "MedicalRecords",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
