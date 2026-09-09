namespace backend.Controllers;

public enum Penalty {
    None,
    Plus2,
    DNF,
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
