using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolvesController(AppDbContext context, StatisticsService statistics) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetSolves() {
        var solves = await context.Solves
            .OrderByDescending(s => s.TimeSolved)
            .ToListAsync();

        return Ok(solves);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSolve([FromRoute] int id) {
        var solve = await context.Solves.FindAsync(id);

        if (solve == null) {
            return NotFound();
        }

        return Ok(solve);
    }

    [HttpPost]
    public async Task<IActionResult> PostSolve([FromBody] PostSolveRequest req) {
        string scramble = req.Scramble;
        Penalty penalty = req.Penalty;
        int solveTime = req.SolveTime;
        int SessionId = req.SessionId;

        if (scramble.Length == 0) {
            Console.WriteLine("Scramble cannot be an empty string");
            return BadRequest();
        }

        if (solveTime < 0) {
            Console.WriteLine("Solve time cannot be negative");
            return BadRequest();
        }

        var solve = new Solve {
            Scramble = scramble,
            Penalty = penalty,
            SolveTime = solveTime,
            SessionId = SessionId,
            TimeSolved = DateTime.UtcNow,
            Type = SolveType.Scramble,
        };

        context.Solves.Add(solve);
        await context.SaveChangesAsync();

        return Created(nameof(GetSolve), solve);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSolve([FromRoute] int id, [FromBody] PostSolveRequest req) {
        var solve = await context.Solves.FindAsync(id);

        if (solve == null) {
            return NotFound();
        }

        solve.Scramble = req.Scramble;
        solve.Penalty = req.Penalty;
        solve.SolveTime = req.SolveTime;
        solve.TimeSolved = DateTime.UtcNow;

        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteAllSolves() {
        context.Solves.RemoveRange(context.Solves);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSolve([FromRoute] int id) {
        var solve = await context.Solves.FindAsync(id);

        if (solve == null) {
            return NotFound();
        }

        context.Solves.Remove(solve);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("statistics/{sessionId}")]
    public async Task<IActionResult> GetStatistics([FromRoute] int sessionId) {
        var solves = await context.Solves.Where(s => s.SessionId == sessionId).ToListAsync();

        var response = new StatisticsResponse {
            TotalAverage = statistics.CalculateTotalAverage(solves),
            Ao5 = statistics.CalculateAo5(solves),
            Ao12 = statistics.CalculateAo12(solves),
            Ao50 = statistics.CalculateAo50(solves),
            Ao100 = statistics.CalculateAo100(solves),
            PersonalBest = statistics.GetPersonalBest(solves),
        };

        return Ok(response);
    }
}

public class PostSolveRequest {
    public required string Scramble { get; set; } = "";
    public required Penalty Penalty { get; set; }
    public required int SolveTime { get; set; }
    public required int SessionId { get; set; }
}

public class StatisticsResponse {
    public double? TotalAverage { get; set; }
    public double? Ao5 { get; set; }
    public double? Ao12 { get; set; }
    public double? Ao50 { get; set; }
    public double? Ao100 { get; set; }
    public double? PersonalBest { get; set; }
}
