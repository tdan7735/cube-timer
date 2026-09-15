using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolvesController(SolveContext context) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetSolves() {
        var solves = await context.Solves
            .OrderByDescending(s => s.TimeSolved)
            .ToListAsync();

        return Ok(solves);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetSolves([FromRoute] int id) {
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
            TimeSolved = DateTime.UtcNow,
        };

        context.Solves.Add(solve);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(PostSolve), new { solve.Id }, solve);
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
}

public class PostSolveRequest {
    public required string Scramble { get; set; } = "";
    public required Penalty Penalty { get; set; }
    public required int SolveTime { get; set; }
}
