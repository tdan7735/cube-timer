using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class AddAlgorithms1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AlgorithmSets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmSets", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "AlgorithmCases",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    AlgorithmSetId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AlgorithmCases", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AlgorithmCases_AlgorithmSets_AlgorithmSetId",
                        column: x => x.AlgorithmSetId,
                        principalTable: "AlgorithmSets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Algorithms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Moves = table.Column<string>(type: "text", nullable: false),
                    AlgorithmCaseId = table.Column<int>(type: "integer", nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Algorithms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Algorithms_AlgorithmCases_AlgorithmCaseId",
                        column: x => x.AlgorithmCaseId,
                        principalTable: "AlgorithmCases",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AlgorithmCases_AlgorithmSetId",
                table: "AlgorithmCases",
                column: "AlgorithmSetId");

            migrationBuilder.CreateIndex(
                name: "IX_Algorithms_AlgorithmCaseId",
                table: "Algorithms",
                column: "AlgorithmCaseId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Algorithms");

            migrationBuilder.DropTable(
                name: "AlgorithmCases");

            migrationBuilder.DropTable(
                name: "AlgorithmSets");
        }
    }
}
