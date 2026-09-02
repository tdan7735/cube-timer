using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

public enum Penalty {
    DNF,
    Plus2,
    None,
    DNS,
}

public class Solve {
    public int Id { get; set; }
    public string Scramble { get; set; } = "";
    public Penalty Penalty { get; set; }
    public int SolveTime { get; set; }    // in milliseconds without penalty
    public DateTime TimeSolved { get; set; }

    public int FinalTime() {
        if (Penalty == Penalty.Plus2) {
            return SolveTime + 2000;
        }
        else {
            return SolveTime;
        }
    }
}

public class SolveContext : DbContext {
    public DbSet<Solve> Solves { get; set; }

    public string DbPath { get; set; }

    public SolveContext() {
        var folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);

        DbPath = Path.Combine(path, "cube-timer.db");
        Console.WriteLine(DbPath);
        Console.WriteLine(DbPath);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
