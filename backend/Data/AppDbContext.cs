namespace backend.Data;

using backend.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options) {
    public DbSet<Solve> Solves { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
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
    }

    public DbSet<AlgorithmSet> AlgorithmSets { get; set; }        // OLL, PLL, etc.
    public DbSet<AlgorithmGroup> AlgorithmGroups { get; set; }    // Dot Case, Fish Shapes, etc.
    public DbSet<AlgorithmCase> AlgorithmCases { get; set; }      // Aa, Ab, F, etc.
    public DbSet<Algorithm> Algorithms { get; set; }              // The actual algorithm itself
}
