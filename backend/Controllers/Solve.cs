namespace backend.Controllers;

public enum Penalty {
    DNF,
    Plus2,
    None,
}

public class Solve(
        int solveTime,
        string scramble,
        Penalty penalty,
        DateTime when) {

    // private values
    // private readonly int id;
    // private readonly int solveTime; // in milliseconds
    // private readonly string scramble;
    // private readonly Penalty penalty;
    // private readonly DateTime when;
    //
    // // Constructor
    // public Solve(int solveTime, string scramble, Penalty penalty) {
    //     this.solveTime = solveTime;
    //     this.scramble = scramble;
    //     this.penalty = penalty;
    //     when = DateTime.Now;
    //
    //     id = DateTime.Now.Millisecond;
    // }

    // Getters
    public int GetSolveTime() { return solveTime; }
    public string GetScramble() { return scramble; }
    public Penalty GetPenalty() { return penalty; }
    public DateTime GetWhen() { return when; }
}

