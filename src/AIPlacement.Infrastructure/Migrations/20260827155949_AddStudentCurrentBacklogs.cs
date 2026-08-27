using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AIPlacement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentCurrentBacklogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_StudentProfiles_StudentId",
                table: "Certifications");

            migrationBuilder.AddColumn<int>(
                name: "CurrentBacklogs",
                table: "StudentProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_StudentProfiles_StudentId",
                table: "Certifications",
                column: "StudentId",
                principalTable: "StudentProfiles",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Certifications_StudentProfiles_StudentId",
                table: "Certifications");

            migrationBuilder.DropColumn(
                name: "CurrentBacklogs",
                table: "StudentProfiles");

            migrationBuilder.AddForeignKey(
                name: "FK_Certifications_StudentProfiles_StudentId",
                table: "Certifications",
                column: "StudentId",
                principalTable: "StudentProfiles",
                principalColumn: "StudentId",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
