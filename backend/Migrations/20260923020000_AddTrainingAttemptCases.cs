using backend.Data;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260923020000_AddTrainingAttemptCases")]
    public partial class AddTrainingAttemptCases : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlgorithmCaseId",
                table: "Solves",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Solves_AlgorithmCaseId",
                table: "Solves",
                column: "AlgorithmCaseId");

            migrationBuilder.AddForeignKey(
                name: "FK_Solves_AlgorithmCases_AlgorithmCaseId",
                table: "Solves",
                column: "AlgorithmCaseId",
                principalTable: "AlgorithmCases",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Solves_AlgorithmCases_AlgorithmCaseId",
                table: "Solves");

            migrationBuilder.DropIndex(
                name: "IX_Solves_AlgorithmCaseId",
                table: "Solves");

            migrationBuilder.DropColumn(
                name: "AlgorithmCaseId",
                table: "Solves");
        }
    }
}
