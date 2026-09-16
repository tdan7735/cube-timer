using backend.Models;
using backend.Services;

namespace backend.Tests;

public class StatisticsServiceTests {
    private readonly StatisticsService _service = new();

    private static Solve CreateSolve(int solveTime, Penalty penalty = Penalty.None, int minutesAgo = 0) {
        return new Solve {
            Scramble = "R U R' U'",
            Penalty = penalty,
            SolveTime = solveTime,
            TimeSolved = DateTime.UtcNow.AddMinutes(-minutesAgo),
        };
    }

    // --- CalculateTotalAverage tests ---

    [Fact]
    public void CalculateTotalAverage_EmptyList_ReturnsNull() {
        var result = _service.CalculateTotalAverage(new List<Solve>());
        Assert.Null(result);
    }

    [Fact]
    public void CalculateTotalAverage_SingleSolve_ReturnsSolveTime() {
        var solves = new List<Solve> { CreateSolve(5000) };
        var result = _service.CalculateTotalAverage(solves);
        Assert.Equal(5000, result);
    }

    [Fact]
    public void CalculateTotalAverage_MultipleSolves_ReturnsAverage() {
        var solves = new List<Solve> {
            CreateSolve(1000),
            CreateSolve(2000),
            CreateSolve(3000),
        };
        var result = _service.CalculateTotalAverage(solves);
        Assert.Equal(2000, result);
    }

    [Fact]
    public void CalculateTotalAverage_WithDnf_ExcludesDnfFromAverage() {
        var solves = new List<Solve> {
            CreateSolve(1000),
            CreateSolve(2000),
            CreateSolve(3000, Penalty.DNF),
        };
        var result = _service.CalculateTotalAverage(solves);
        Assert.Equal(1500, result);
    }

    [Fact]
    public void CalculateTotalAverage_AllDnf_ReturnsNan() {
        var solves = new List<Solve> {
            CreateSolve(1000, Penalty.DNF),
            CreateSolve(2000, Penalty.DNF),
        };
        var result = _service.CalculateTotalAverage(solves);
        Assert.NotNull(result);
        Assert.True(double.IsNaN(result.Value));
    }

    [Fact]
    public void CalculateTotalAverage_WithPlus2_UsesFinalTime() {
        var solves = new List<Solve> {
            CreateSolve(1000),
            CreateSolve(2000, Penalty.Plus2),
        };
        var result = _service.CalculateTotalAverage(solves);
        Assert.Equal(2500, result);
    }

    // --- CalculateAo5 tests ---

    [Fact]
    public void CalculateAo5_LessThan5Solves_ReturnsNull() {
        var solves = new List<Solve> {
            CreateSolve(1000),
            CreateSolve(2000),
            CreateSolve(3000),
            CreateSolve(4000),
        };
        var result = _service.CalculateAo5(solves);
        Assert.Null(result);
    }

    [Fact]
    public void CalculateAo5_Exactly5Solves_TrimsFastestAndSlowest() {
        var solves = new List<Solve> {
            CreateSolve(1000, minutesAgo: 4),
            CreateSolve(2000, minutesAgo: 3),
            CreateSolve(3000, minutesAgo: 2),
            CreateSolve(4000, minutesAgo: 1),
            CreateSolve(5000, minutesAgo: 0),
        };
        var result = _service.CalculateAo5(solves);
        Assert.Equal(3000, result);
    }

    [Fact]
    public void CalculateAo5_DnfInTrimmedPosition_IgnoresIt() {
        var solves = new List<Solve> {
            CreateSolve(1000, Penalty.DNF, minutesAgo: 4),
            CreateSolve(2000, minutesAgo: 3),
            CreateSolve(3000, minutesAgo: 2),
            CreateSolve(4000, minutesAgo: 1),
            CreateSolve(5000, minutesAgo: 0),
        };
        var result = _service.CalculateAo5(solves);
        Assert.Equal(4000, result);
    }

    [Fact]
    public void CalculateAo5_WithDnfInMiddle_ReturnsNegativeOne() {
        var solves = new List<Solve> {
            CreateSolve(1000, minutesAgo: 4),
            CreateSolve(2000, Penalty.DNF, minutesAgo: 3),
            CreateSolve(3000, Penalty.DNF, minutesAgo: 2),
            CreateSolve(4000, minutesAgo: 1),
            CreateSolve(5000, minutesAgo: 0),
        };
        var result = _service.CalculateAo5(solves);
        Assert.Equal(-1, result);
    }

    [Fact]
    public void CalculateAo5_TakesMostRecent5() {
        var solves = new List<Solve> {
            CreateSolve(1000, minutesAgo: 10),
            CreateSolve(2000, minutesAgo: 9),
            CreateSolve(3000, minutesAgo: 8),
            CreateSolve(4000, minutesAgo: 7),
            CreateSolve(5000, minutesAgo: 6),
            CreateSolve(1500, minutesAgo: 5),
            CreateSolve(2500, minutesAgo: 4),
            CreateSolve(3500, minutesAgo: 3),
            CreateSolve(4500, minutesAgo: 2),
            CreateSolve(5500, minutesAgo: 1),
        };
        var result = _service.CalculateAo5(solves);
        Assert.Equal(3500, result);
    }

    [Fact]
    public void CalculateAo5_WithPlus2Penalty_UsesFinalTime() {
        var solves = new List<Solve> {
            CreateSolve(1000, minutesAgo: 4),
            CreateSolve(2000, minutesAgo: 3),
            CreateSolve(3000, Penalty.Plus2, minutesAgo: 2),
            CreateSolve(4000, minutesAgo: 1),
            CreateSolve(5000, minutesAgo: 0),
        };
        var result = _service.CalculateAo5(solves);
        Assert.Equal(3666.6666666666665, result);
    }

    // --- CalculateAo12 tests ---

    [Fact]
    public void CalculateAo12_LessThan12Solves_ReturnsNull() {
        var solves = Enumerable.Range(1, 11)
            .Select(i => CreateSolve(i * 1000))
            .ToList();
        var result = _service.CalculateAo12(solves);
        Assert.Null(result);
    }

    [Fact]
    public void CalculateAo12_Exactly12Solves_TrimsFastestAndSlowest() {
        var solves = Enumerable.Range(1, 12)
            .Select(i => CreateSolve(i * 1000, minutesAgo: 12 - i))
            .ToList();
        var result = _service.CalculateAo12(solves);
        var expected = Enumerable.Range(2, 10).Average() * 1000;
        Assert.Equal(expected, result);
    }

    [Fact]
    public void CalculateAo12_WithDnfInMiddle_ReturnsNegativeOne() {
        var solves = Enumerable.Range(1, 12)
            .Select(i => CreateSolve(i * 1000, minutesAgo: 12 - i))
            .ToList();
        solves[0].Penalty = Penalty.DNF;
        solves[1].Penalty = Penalty.DNF;
        var result = _service.CalculateAo12(solves);
        Assert.Equal(-1, result);
    }

    // --- CalculateAo50 tests ---

    [Fact]
    public void CalculateAo50_LessThan50Solves_ReturnsNull() {
        var solves = Enumerable.Range(1, 49)
            .Select(i => CreateSolve(i * 1000))
            .ToList();
        var result = _service.CalculateAo50(solves);
        Assert.Null(result);
    }

    [Fact]
    public void CalculateAo50_Exactly50Solves_TrimsTopAndBottom3() {
        var solves = Enumerable.Range(1, 50)
            .Select(i => CreateSolve(i * 1000, minutesAgo: 50 - i))
            .ToList();
        var result = _service.CalculateAo50(solves);
        var expected = Enumerable.Range(4, 44).Average() * 1000;
        Assert.Equal(expected, result);
    }

    // --- CalculateAo100 tests ---

    [Fact]
    public void CalculateAo100_LessThan100Solves_ReturnsNull() {
        var solves = Enumerable.Range(1, 99)
            .Select(i => CreateSolve(i * 1000))
            .ToList();
        var result = _service.CalculateAo100(solves);
        Assert.Null(result);
    }

    [Fact]
    public void CalculateAo100_Exactly100Solves_TrimsTopAndBottom5() {
        var solves = Enumerable.Range(1, 100)
            .Select(i => CreateSolve(i * 1000, minutesAgo: 100 - i))
            .ToList();
        var result = _service.CalculateAo100(solves);
        var expected = Enumerable.Range(6, 90).Average() * 1000;
        Assert.Equal(expected, result);
    }

    // --- GetPersonalBest tests ---

    [Fact]
    public void GetPersonalBest_EmptyList_ReturnsNull() {
        var result = _service.GetPersonalBest(new List<Solve>());
        Assert.Null(result);
    }

    [Fact]
    public void GetPersonalBest_SingleSolve_ReturnsFinalTime() {
        var solves = new List<Solve> { CreateSolve(5000) };
        var result = _service.GetPersonalBest(solves);
        Assert.Equal(5000, result);
    }

    [Fact]
    public void GetPersonalBest_MultipleSolves_ReturnsFastest() {
        var solves = new List<Solve> {
            CreateSolve(5000),
            CreateSolve(3000),
            CreateSolve(7000),
        };
        var result = _service.GetPersonalBest(solves);
        Assert.Equal(3000, result);
    }

    [Fact]
    public void GetPersonalBest_WithDnf_ReturnsFastestNonDnf() {
        var solves = new List<Solve> {
            CreateSolve(5000, Penalty.DNF),
            CreateSolve(3000),
            CreateSolve(7000),
        };
        var result = _service.GetPersonalBest(solves);
        Assert.Equal(3000, result);
    }

    [Fact]
    public void GetPersonalBest_WithPlus2_UsesFinalTime() {
        var solves = new List<Solve> {
            CreateSolve(3000, Penalty.Plus2),
            CreateSolve(4000),
        };
        var result = _service.GetPersonalBest(solves);
        Assert.Equal(4000, result);
    }

    [Fact]
    public void GetPersonalBest_AllDnf_ReturnsFastestFinalTime() {
        var solves = new List<Solve> {
            CreateSolve(3000, Penalty.DNF),
            CreateSolve(4000, Penalty.DNF),
        };
        var result = _service.GetPersonalBest(solves);
        Assert.Equal(3000, result);
    }
}
