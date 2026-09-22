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
    public async Task<IActionResult> GetAllSolves() {
        var user = await context
            .Users
            .Where(u => u.Username == UserSeeder.DefaultUsername)
            .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var solves = await context.Solves
            .Where(s => s.Session != null && s.Session.UserId == user.Id)
            .OrderByDescending(s => s.TimeSolved)
            .ToListAsync();

        return Ok(solves);
    }

    [HttpGet("session/{sessionId}")]
    public async Task<IActionResult> GetSolves([FromRoute] int sessionId) {
        var user = await context
            .Users
            .Where(u => u.Username == UserSeeder.DefaultUsername)
            .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var sessionExists = await context.Sessions
            .AnyAsync(s => s.Id == sessionId && s.UserId == user.Id);
        if (!sessionExists) {
            return NotFound("Session not found");
        }

        var solves = await context.Solves
            .Where(s => s.SessionId == sessionId)
            .OrderByDescending(s => s.TimeSolved)
            .ToListAsync();

        return Ok(solves);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSolve([FromRoute] int id) {
        var user = await context
                .Users
                .Where(u => u.Username == UserSeeder.DefaultUsername)
                .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var solve = await context.Solves
            .SingleOrDefaultAsync(s => s.Id == id && s.Session != null && s.Session.UserId == user.Id);
        if (solve == null) {
            return NotFound("User does not own this solve");
        }

        return Ok(solve);
    }

    [HttpPost]
    public async Task<IActionResult> PostSolve([FromBody] PostSolveRequest req) {
        var user = await context
            .Users
            .Where(u => u.Username == UserSeeder.DefaultUsername)
            .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        string scramble = req.Scramble;
        Penalty penalty = req.Penalty;
        int solveTime = req.SolveTime;
        if (scramble.Length == 0) {
            return BadRequest("Scramble must have a length greater than 0");
        }

        if (solveTime < 0) {
            return BadRequest("Solve time cannot be negative");
        }

        var session = await context.Sessions
            .SingleOrDefaultAsync(s => s.Id == req.SessionId && s.UserId == user.Id);
        if (session == null) {
            return NotFound("Session not found");
        }

        var solve = new Solve {
            Scramble = scramble,
            Penalty = penalty,
            SolveTime = solveTime,
            SessionId = session.Id,
            TimeSolved = DateTime.UtcNow,
            Type = SolveType.Scramble,
        };

        context.Solves.Add(solve);
        await context.SaveChangesAsync();

        return Created(nameof(GetSolve), solve);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateSolve([FromRoute] int id, [FromBody] PostSolveRequest req) {
        var user = await context
            .Users
            .Where(u => u.Username == UserSeeder.DefaultUsername)
            .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var solve = await context.Solves
            .SingleOrDefaultAsync(s => s.Id == id && s.Session != null && s.Session.UserId == user.Id);
        if (solve == null) {
            return NotFound("User does not own this solve");
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
        var user = await context
            .Users
            .Where(u => u.Username == UserSeeder.DefaultUsername)
            .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        context.Solves.RemoveRange(
                context.Solves.Where(s => s.Session == null || s.Session.UserId == user.Id));

        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteSolve([FromRoute] int id) {
        var user = await context
            .Users
            .Where(u => u.Username == UserSeeder.DefaultUsername)
            .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var solve = await context.Solves
            .SingleOrDefaultAsync(s => s.Id == id && s.Session != null && s.Session.UserId == user.Id);
        if (solve == null) {
            return NotFound("User does not own this solve");
        }

        context.Solves.Remove(solve);
        await context.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("statistics/{sessionId}")]
    public async Task<IActionResult> GetStatistics([FromRoute] int sessionId) {
        var user = await context
            .Users
            .Where(u => u.Username == UserSeeder.DefaultUsername)
            .FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var sessionExists = await context.Sessions
            .AnyAsync(s => s.Id == sessionId && s.UserId == user.Id);
        if (!sessionExists) {
            return NotFound("Session not found");
        }

        var solves = await context.Solves
            .Where(s => s.SessionId == sessionId)
            .ToListAsync();

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
