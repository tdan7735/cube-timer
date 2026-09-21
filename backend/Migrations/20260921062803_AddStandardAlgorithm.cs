using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddStandardAlgorithm : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Algorithms_AlgorithmCaseId",
                table: "Algorithms");

            migrationBuilder.AddColumn<bool>(
                name: "IsStandard",
                table: "Algorithms",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            // Existing seeds insert the standard first for each case.
            migrationBuilder.Sql("""
                UPDATE "Algorithms"
                SET "IsStandard" = true
                WHERE "Id" IN (
                    SELECT MIN("Id") FROM "Algorithms" GROUP BY "AlgorithmCaseId"
                );
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Algorithms_StandardPerCase",
                table: "Algorithms",
                column: "AlgorithmCaseId",
                unique: true,
                filter: "\"IsStandard\" = true");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Algorithms_StandardPerCase",
                table: "Algorithms");

            migrationBuilder.DropColumn(
                name: "IsStandard",
                table: "Algorithms");

            migrationBuilder.CreateIndex(
                name: "IX_Algorithms_AlgorithmCaseId",
                table: "Algorithms",
                column: "AlgorithmCaseId");
        }
    }
}
