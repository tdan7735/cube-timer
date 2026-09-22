using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class StuffForTraining : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlgorithmSetId",
                table: "Sessions",
                type: "integer",
                nullable: true);

            migrationBuilder.DropIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Sessions_TrainingSession_AlgorithmSet",
                table: "Sessions",
                sql: "\"Type\" <> 1 OR \"AlgorithmSetId\" IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_AlgorithmSetId",
                table: "Sessions",
                column: "AlgorithmSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId_AlgorithmSetId",
                table: "Sessions",
                columns: new[] { "UserId", "AlgorithmSetId" },
                unique: true,
                filter: "\"Type\" = 1 AND \"AlgorithmSetId\" IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_Sessions_AlgorithmSets_AlgorithmSetId",
                table: "Sessions",
                column: "AlgorithmSetId",
                principalTable: "AlgorithmSets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sessions_AlgorithmSets_AlgorithmSetId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_UserId_AlgorithmSetId",
                table: "Sessions");

            migrationBuilder.DropIndex(
                name: "IX_Sessions_AlgorithmSetId",
                table: "Sessions");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Sessions_TrainingSession_AlgorithmSet",
                table: "Sessions");

            migrationBuilder.CreateIndex(
                name: "IX_Sessions_UserId",
                table: "Sessions",
                column: "UserId");

            migrationBuilder.DropColumn(
                name: "AlgorithmSetId",
                table: "Sessions");
        }
    }
}
