namespace backend.Models;

public enum Penalty {
    None,
    Plus2,
    DNF,
}

public enum SolveType {
    PLL,
    OLL,
    Scramble,
}

public class Solve {
    public int Id { get; set; }
    public required string Scramble { get; set; } = "";
    public required Penalty Penalty { get; set; }
    public required int SolveTime { get; set; }    // in milliseconds without penalty
    public required DateTime TimeSolved { get; set; }
    public required SolveType Type { get; set; }

    public int FinalTime() {
        if (Penalty == Penalty.Plus2) {
            return SolveTime + 2000;
        }
        else {
            return SolveTime;
        }
    }
}
