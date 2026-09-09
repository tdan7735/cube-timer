using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolvesController(SolveContext context) : ControllerBase {

    [HttpGet]
    public async Task<IActionResult> GetSolves() {
        var solves = await context.Solves.ToListAsync();
        return Ok(solves);
    }

    [HttpPost]
    public async Task<IActionResult> PostSolve(
            [FromForm] string scramble,
            [FromForm] string penalty,
            [FromForm] int solveTime) {
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

        var solve = new Solve {
            Scramble = scramble,
            Penalty = solvePenalty,
            SolveTime = solveTime,
            TimeSolved = DateTime.UtcNow,
        };
        context.Solves.Add(solve);
        await context.SaveChangesAsync();
        return Ok();
    }
}
