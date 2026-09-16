using backend.Models;

namespace backend.Tests;

public class SolveTests {

    [Fact]
    public void FinalTime_NoPenalty_ReturnsSolveTime() {
        var solve = new Solve {
            Scramble = "R U R' U'",
            Penalty = Penalty.None,
            SolveTime = 5000,
            TimeSolved = DateTime.UtcNow,
        };

        Assert.Equal(5000, solve.FinalTime());
    }

    [Fact]
    public void FinalTime_Plus2_ReturnsSolveTimePlus2000() {
        var solve = new Solve {
            Scramble = "R U R' U'",
            Penalty = Penalty.Plus2,
            SolveTime = 5000,
            TimeSolved = DateTime.UtcNow,
        };

        Assert.Equal(7000, solve.FinalTime());
    }

    [Fact]
    public void FinalTime_DNF_ReturnsSolveTime() {
        var solve = new Solve {
            Scramble = "R U R' U'",
            Penalty = Penalty.DNF,
            SolveTime = 5000,
            TimeSolved = DateTime.UtcNow,
        };

        Assert.Equal(5000, solve.FinalTime());
    }

    [Fact]
    public void FinalTime_ZeroTime_ReturnsZero() {
        var solve = new Solve {
            Scramble = "R U R' U'",
            Penalty = Penalty.None,
            SolveTime = 0,
            TimeSolved = DateTime.UtcNow,
        };

        Assert.Equal(0, solve.FinalTime());
    }

    [Fact]
    public void FinalTime_Plus2WithZeroTime_Returns2000() {
        var solve = new Solve {
            Scramble = "R U R' U'",
            Penalty = Penalty.Plus2,
            SolveTime = 0,
            TimeSolved = DateTime.UtcNow,
        };

        Assert.Equal(2000, solve.FinalTime());
    }
}
