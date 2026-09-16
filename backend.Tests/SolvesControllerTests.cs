using backend.Controllers;
using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.EntityFrameworkCore;

namespace backend.Tests;

public class SolvesControllerTests {
    private readonly StatisticsService _statistics = new();

    private static SolveContext CreateInMemoryContext() {
        var options = new DbContextOptionsBuilder<SolveContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        return new SolveContext(options);
    }

    private static async Task SeedDatabase(SolveContext context, List<Solve> solves) {
        context.Solves.AddRange(solves);
        await context.SaveChangesAsync();
    }

    // --- GET /api/solves ---

    [Fact]
    public async Task GetSolves_EmptyDatabase_ReturnsOkEmptyList() {
        using var context = CreateInMemoryContext();
        var controller = new SolvesController(context, _statistics);

        var result = await controller.GetSolves();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var solves = Assert.IsAssignableFrom<List<Solve>>(okResult.Value);
        Assert.Empty(solves);
    }

    [Fact]
    public async Task GetSolves_WithSolves_ReturnsOkWithSolves() {
        using var context = CreateInMemoryContext();
        await SeedDatabase(context, new List<Solve> {
            new() { Scramble = "R U", Penalty = Penalty.None, SolveTime = 5000, TimeSolved = DateTime.UtcNow },
            new() { Scramble = "R' U'", Penalty = Penalty.None, SolveTime = 3000, TimeSolved = DateTime.UtcNow.AddMinutes(-1) },
        });
        var controller = new SolvesController(context, _statistics);

        var result = await controller.GetSolves();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var solves = Assert.IsAssignableFrom<List<Solve>>(okResult.Value);
        Assert.Equal(2, solves.Count);
    }

    // --- GET /api/solves/{id} ---

    [Fact]
    public async Task GetSolve_ExistingId_ReturnsOkWithSolve() {
        using var context = CreateInMemoryContext();
        var solve = new Solve { Id = 1, Scramble = "R U", Penalty = Penalty.None, SolveTime = 5000, TimeSolved = DateTime.UtcNow };
        await SeedDatabase(context, new List<Solve> { solve });
        var controller = new SolvesController(context, _statistics);

        var result = await controller.GetSolve(1);

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var returned = Assert.IsType<Solve>(okResult.Value);
        Assert.Equal(5000, returned.SolveTime);
    }

    [Fact]
    public async Task GetSolve_NonexistentId_ReturnsNotFound() {
        using var context = CreateInMemoryContext();
        var controller = new SolvesController(context, _statistics);

        var result = await controller.GetSolve(999);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    // --- POST /api/solves ---

    [Fact]
    public async Task PostSolve_ValidRequest_CreatesSolve() {
        using var context = CreateInMemoryContext();
        var controller = new SolvesController(context, _statistics);
        var request = new PostSolveRequest {
            Scramble = "R U R' U'",
            Penalty = Penalty.None,
            SolveTime = 5000,
        };

        var result = await controller.PostSolve(request);

        var createdResult = Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedResult>(result);
        var solve = Assert.IsType<Solve>(createdResult.Value);
        Assert.Equal("R U R' U'", solve.Scramble);
        Assert.Equal(5000, solve.SolveTime);
        Assert.Equal(Penalty.None, solve.Penalty);
    }

    [Fact]
    public async Task PostSolve_EmptyScramble_ReturnsBadRequest() {
        using var context = CreateInMemoryContext();
        var controller = new SolvesController(context, _statistics);
        var request = new PostSolveRequest {
            Scramble = "",
            Penalty = Penalty.None,
            SolveTime = 5000,
        };

        var result = await controller.PostSolve(request);

        Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result);
    }

    [Fact]
    public async Task PostSolve_NegativeSolveTime_ReturnsBadRequest() {
        using var context = CreateInMemoryContext();
        var controller = new SolvesController(context, _statistics);
        var request = new PostSolveRequest {
            Scramble = "R U R' U'",
            Penalty = Penalty.None,
            SolveTime = -100,
        };

        var result = await controller.PostSolve(request);

        Assert.IsType<Microsoft.AspNetCore.Mvc.BadRequestResult>(result);
    }

    [Fact]
    public async Task PostSolve_ZeroSolveTime_CreatesSolve() {
        using var context = CreateInMemoryContext();
        var controller = new SolvesController(context, _statistics);
        var request = new PostSolveRequest {
            Scramble = "R U R' U'",
            Penalty = Penalty.None,
            SolveTime = 0,
        };

        var result = await controller.PostSolve(request);

        Assert.IsType<Microsoft.AspNetCore.Mvc.CreatedResult>(result);
    }

    // --- DELETE /api/solves/{id} ---

    [Fact]
    public async Task DeleteSolve_ExistingId_ReturnsOk() {
        using var context = CreateInMemoryContext();
        await SeedDatabase(context, new List<Solve> {
            new() { Id = 1, Scramble = "R U", Penalty = Penalty.None, SolveTime = 5000, TimeSolved = DateTime.UtcNow },
        });
        var controller = new SolvesController(context, _statistics);

        var result = await controller.DeleteSolve(1);

        Assert.IsType<Microsoft.AspNetCore.Mvc.OkResult>(result);
        Assert.Null(await context.Solves.FindAsync(1));
    }

    [Fact]
    public async Task DeleteSolve_NonexistentId_ReturnsNotFound() {
        using var context = CreateInMemoryContext();
        var controller = new SolvesController(context, _statistics);

        var result = await controller.DeleteSolve(999);

        Assert.IsType<Microsoft.AspNetCore.Mvc.NotFoundResult>(result);
    }

    // --- DELETE /api/solves ---

    [Fact]
    public async Task DeleteAllSolves_ReturnsOkAndClearsDatabase() {
        using var context = CreateInMemoryContext();
        await SeedDatabase(context, new List<Solve> {
            new() { Id = 1, Scramble = "R U", Penalty = Penalty.None, SolveTime = 5000, TimeSolved = DateTime.UtcNow },
            new() { Id = 2, Scramble = "R' U'", Penalty = Penalty.None, SolveTime = 3000, TimeSolved = DateTime.UtcNow },
        });
        var controller = new SolvesController(context, _statistics);

        var result = await controller.DeleteAllSolves();

        Assert.IsType<Microsoft.AspNetCore.Mvc.OkResult>(result);
        Assert.Empty(await context.Solves.ToListAsync());
    }

    // --- GET /api/solves/statistics ---

    [Fact]
    public async Task GetStatistics_EmptyDatabase_ReturnsOkWithNulls() {
        using var context = CreateInMemoryContext();
        var controller = new SolvesController(context, _statistics);

        var result = await controller.GetStatistics();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var stats = Assert.IsType<StatisticsResponse>(okResult.Value);
        Assert.Null(stats.TotalAverage);
        Assert.Null(stats.Ao5);
        Assert.Null(stats.Ao12);
        Assert.Null(stats.Ao50);
        Assert.Null(stats.Ao100);
        Assert.Null(stats.PersonalBest);
    }

    [Fact]
    public async Task GetStatistics_WithSolves_ReturnsOkWithStatistics() {
        using var context = CreateInMemoryContext();
        var solves = Enumerable.Range(1, 12)
            .Select(i => new Solve {
                Scramble = $"R U{i}",
                Penalty = Penalty.None,
                SolveTime = i * 1000,
                TimeSolved = DateTime.UtcNow.AddMinutes(-12 + i),
            })
            .ToList();
        await SeedDatabase(context, solves);
        var controller = new SolvesController(context, _statistics);

        var result = await controller.GetStatistics();

        var okResult = Assert.IsType<Microsoft.AspNetCore.Mvc.OkObjectResult>(result);
        var stats = Assert.IsType<StatisticsResponse>(okResult.Value);
        Assert.NotNull(stats.TotalAverage);
        Assert.NotNull(stats.Ao5);
        Assert.NotNull(stats.Ao12);
        Assert.NotNull(stats.PersonalBest);
    }
}
