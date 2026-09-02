using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolvesController : ControllerBase {


    [HttpGet]
    public IActionResult GetSolves() {
        using var db = new SolveContext();
        return Ok(db.Solves);
    }

    [HttpPost]
    public IActionResult PostSolve(string scramble, string penalty, int solveTime) {
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
        using var db = new SolveContext();
        db.Solves.Add(solve);
        db.SaveChanges();
        return Ok();
    }
}
