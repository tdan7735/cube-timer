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
        string penalty = req.Penalty;
        int solveTime = req.SolveTime;

        Penalty solvePenalty;
        if (penalty == "DNF") {
            solvePenalty = Penalty.DNF;
        }
        else if (penalty == "Plus2") {
            solvePenalty = Penalty.Plus2;
        }
        else if (penalty == "None") {
            solvePenalty = Penalty.None;
        }
        else {
            Console.WriteLine("Invalid penalty");
            return BadRequest();
        }

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
            Penalty = solvePenalty,
            SolveTime = solveTime,
            TimeSolved = DateTime.UtcNow,
        };

        context.Solves.Add(solve);
        await context.SaveChangesAsync();

        return CreatedAtAction(nameof(PostSolve), new { solve.Id }, solve);
    }
}

public class PostSolveRequest {
    public string Scramble { get; set; } = "";
    public string Penalty { get; set; } = "";
    public int SolveTime { get; set; }
}
