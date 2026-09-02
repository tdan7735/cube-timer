using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolvesController : ControllerBase {
    private readonly SolveContext _context;

    public SolvesController(SolveContext context) {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> GetSolves() {
        var solves = await _context.Solves.ToListAsync();
        return Ok(solves);
    }

    [HttpPost]
    public async Task<IActionResult> PostSolve(string scramble, string penalty, int solveTime) {
        Penalty solvePenalty;
        if (penalty == "DNF") {
            solvePenalty = Penalty.DNF;
        }
        else if (penalty == "DNS") {
            solvePenalty = Penalty.DNS;
        }
        else if (penalty == "Plus2") {
            solvePenalty = Penalty.Plus2;
        }
        else if (penalty == "None") {
            solvePenalty = Penalty.None;
        }
        else {
            return BadRequest();
        }

        var solve = new Solve {
            Scramble = scramble,
            Penalty = solvePenalty,
            SolveTime = solveTime,
            TimeSolved = DateTime.Now,
        };
        _context.Solves.Add(solve);
        await _context.SaveChangesAsync();
        return Ok();
    }
}
