using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace backend.Migrations;

public partial class AddTrainingProgressAndPreferences : Migration {
    protected override void Up(MigrationBuilder migrationBuilder) {
        migrationBuilder.CreateTable(
            name: "TrainingPreferences",
            columns: table => new {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<int>(type: "integer", nullable: false),
                AlgorithmSetId = table.Column<int>(type: "integer", nullable: false),
                IncludeNotLearned = table.Column<bool>(type: "boolean", nullable: false),
                IncludeLearning = table.Column<bool>(type: "boolean", nullable: false),
                IncludeLearned = table.Column<bool>(type: "boolean", nullable: false),
                Focus = table.Column<int>(type: "integer", nullable: false),
                SlowestCount = table.Column<int>(type: "integer", nullable: false),
                Order = table.Column<int>(type: "integer", nullable: false),
                SelectedCaseIds = table.Column<int[]>(type: "integer[]", nullable: true)
            },
            constraints: table => {
                table.PrimaryKey("PK_TrainingPreferences", x => x.Id);
                table.ForeignKey("FK_TrainingPreferences_AlgorithmSets_AlgorithmSetId", x => x.AlgorithmSetId, "AlgorithmSets", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_TrainingPreferences_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateTable(
            name: "UserAlgorithmCaseProgress",
            columns: table => new {
                Id = table.Column<int>(type: "integer", nullable: false)
                    .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                UserId = table.Column<int>(type: "integer", nullable: false),
                AlgorithmCaseId = table.Column<int>(type: "integer", nullable: false),
                Status = table.Column<int>(type: "integer", nullable: false),
                UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
            },
            constraints: table => {
                table.PrimaryKey("PK_UserAlgorithmCaseProgress", x => x.Id);
                table.ForeignKey("FK_UserAlgorithmCaseProgress_AlgorithmCases_AlgorithmCaseId", x => x.AlgorithmCaseId, "AlgorithmCases", "Id", onDelete: ReferentialAction.Cascade);
                table.ForeignKey("FK_UserAlgorithmCaseProgress_Users_UserId", x => x.UserId, "Users", "Id", onDelete: ReferentialAction.Cascade);
            });

        migrationBuilder.CreateIndex("IX_TrainingPreferences_AlgorithmSetId", "TrainingPreferences", "AlgorithmSetId");
        migrationBuilder.CreateIndex("IX_TrainingPreferences_UserId_AlgorithmSetId", "TrainingPreferences", new[] { "UserId", "AlgorithmSetId" }, unique: true);
        migrationBuilder.CreateIndex("IX_UserAlgorithmCaseProgress_AlgorithmCaseId", "UserAlgorithmCaseProgress", "AlgorithmCaseId");
        migrationBuilder.CreateIndex("IX_UserAlgorithmCaseProgress_UserId_AlgorithmCaseId", "UserAlgorithmCaseProgress", new[] { "UserId", "AlgorithmCaseId" }, unique: true);
    }

    protected override void Down(MigrationBuilder migrationBuilder) {
        migrationBuilder.DropTable("TrainingPreferences");
        migrationBuilder.DropTable("UserAlgorithmCaseProgress");
    }
}
