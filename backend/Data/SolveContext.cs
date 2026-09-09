namespace backend.Data;

using backend.Models;
using Microsoft.EntityFrameworkCore;

public class SolveContext(DbContextOptions<SolveContext> options) : DbContext(options) {
    public DbSet<Solve> Solves { get; set; }
}
