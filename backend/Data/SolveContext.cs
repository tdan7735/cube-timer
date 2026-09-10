namespace backend.Data;

using backend.Models;
using Microsoft.EntityFrameworkCore;

public class SolveContext(DbContextOptions<SolveContext> options) : DbContext(options) {
    public DbSet<Solve> Solves { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Solve>()
            .ToTable("Solves", table =>
                    table.HasCheckConstraint(
                        "CK_Solves_SolveTime_NonNegative",
                        "\"SolveTime\" >= 0"
                    )
            );
    }
}
