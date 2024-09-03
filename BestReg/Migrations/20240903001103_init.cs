using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BestReg.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_IllnessTreatmentInfo_TempId",
                table: "IllnessTreatmentInfo");

            migrationBuilder.RenameColumn(
                name: "TempId",
                table: "IllnessTreatmentInfo",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "IllnessTreatmentInfo",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<string>(
                name: "Diagnosis",
                table: "IllnessTreatmentInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Treatment",
                table: "IllnessTreatmentInfo",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_IllnessTreatmentInfo",
                table: "IllnessTreatmentInfo",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_IllnessTreatmentInfo",
                table: "IllnessTreatmentInfo");

            migrationBuilder.DropColumn(
                name: "Diagnosis",
                table: "IllnessTreatmentInfo");

            migrationBuilder.DropColumn(
                name: "Treatment",
                table: "IllnessTreatmentInfo");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "IllnessTreatmentInfo",
                newName: "TempId");

            migrationBuilder.AlterColumn<int>(
                name: "TempId",
                table: "IllnessTreatmentInfo",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_IllnessTreatmentInfo_TempId",
                table: "IllnessTreatmentInfo",
                column: "TempId");
        }
    }
}
