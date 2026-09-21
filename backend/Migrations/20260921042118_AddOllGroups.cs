using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddOllGroups : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AlgorithmGroups_AlgorithmSetId",
                table: "AlgorithmGroups");

            migrationBuilder.DropIndex(
                name: "IX_AlgorithmCases_AlgorithmSetId",
                table: "AlgorithmCases");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmGroups_AlgorithmSetId",
                table: "AlgorithmGroups",
                column: "AlgorithmSetId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmCases_AlgorithmSetId",
                table: "AlgorithmCases",
                column: "AlgorithmSetId");
        }
    }
}
