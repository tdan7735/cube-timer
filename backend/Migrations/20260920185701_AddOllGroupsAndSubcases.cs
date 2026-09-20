using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddOllGroupsAndSubcases : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "AlgorithmGroupId",
                table: "AlgorithmCases",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CaseNumber",
                table: "AlgorithmCases",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AlgorithmGroups",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AlgorithmSetId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmGroups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlgorithmGroups_AlgorithmSets_AlgorithmSetId",
                        column: x => x.AlgorithmSetId,
                        principalTable: "AlgorithmSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmCases_AlgorithmGroupId",
                table: "AlgorithmCases",
                column: "AlgorithmGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmCases_AlgorithmSetId_CaseNumber",
                table: "AlgorithmCases",
                columns: new[] { "AlgorithmSetId", "CaseNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmGroups_AlgorithmSetId_Name",
                table: "AlgorithmGroups",
                columns: new[] { "AlgorithmSetId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmGroups_AlgorithmSetId",
                table: "AlgorithmGroups",
                column: "AlgorithmSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_AlgorithmCases_AlgorithmGroups_AlgorithmGroupId",
                table: "AlgorithmCases",
                column: "AlgorithmGroupId",
                principalTable: "AlgorithmGroups",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AlgorithmCases_AlgorithmGroups_AlgorithmGroupId",
                table: "AlgorithmCases");

            migrationBuilder.DropTable(
                name: "AlgorithmGroups");

            migrationBuilder.DropIndex(
                name: "IX_AlgorithmCases_AlgorithmSetId_CaseNumber",
                table: "AlgorithmCases");

            migrationBuilder.DropIndex(
                name: "IX_AlgorithmCases_AlgorithmGroupId",
                table: "AlgorithmCases");

            migrationBuilder.DropColumn(
                name: "AlgorithmGroupId",
                table: "AlgorithmCases");

            migrationBuilder.DropColumn(
                name: "CaseNumber",
                table: "AlgorithmCases");
        }
    }
}
