namespace backend.Data;

using backend.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<Solve> Solves { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Algorithm>()
            .HasIndex(a => a.AlgorithmCaseId, "IX_Algorithms_StandardPerCase")
            .IsUnique()
            .HasFilter("\"IsStandard\" = true");

        modelBuilder.Entity<Solve>()
            .ToTable("Solves", table =>
                    table.HasCheckConstraint(
                        "CK_Solves_SolveTime_NonNegative",
                        "\"SolveTime\" >= 0"
                    )
            );

        modelBuilder.Entity<AlgorithmGroup>()
            .HasIndex(group => new { group.AlgorithmSetId, group.Name })
            .IsUnique();

        modelBuilder.Entity<AlgorithmCase>()
            .HasIndex(algorithmCase => new { algorithmCase.AlgorithmSetId, algorithmCase.CaseNumber })
            .IsUnique();

        modelBuilder.Entity<Session>()
            .HasIndex(session => new { session.UserId, session.AlgorithmSetId })
            .IsUnique()
            .HasFilter("\"Type\" = 1 AND \"AlgorithmSetId\" IS NOT NULL");

        modelBuilder.Entity<Session>()
            .ToTable("Sessions", table => table.HasCheckConstraint(
                "CK_Sessions_TrainingSession_AlgorithmSet",
                "\"Type\" <> 1 OR \"AlgorithmSetId\" IS NOT NULL"));

        modelBuilder.Entity<UserAlgorithmCaseProgress>()
            .HasIndex(progress => new { progress.UserId, progress.AlgorithmCaseId })
            .IsUnique();

        modelBuilder.Entity<TrainingPreferences>()
            .HasIndex(preferences => new { preferences.UserId, preferences.AlgorithmSetId })
            .IsUnique();
    }

    public DbSet<AlgorithmSet> AlgorithmSets { get; set; }        // OLL, PLL, etc.
    public DbSet<AlgorithmGroup> AlgorithmGroups { get; set; }    // Dot Case, Fish Shapes, etc.
    public DbSet<AlgorithmCase> AlgorithmCases { get; set; }      // Aa, Ab, F, etc.
    public DbSet<Algorithm> Algorithms { get; set; }              // The actual algorithm itself

    public DbSet<Session> Sessions { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<UserAlgorithmCaseProgress> UserAlgorithmCaseProgress { get; set; }
    public DbSet<TrainingPreferences> TrainingPreferences { get; set; }
}
