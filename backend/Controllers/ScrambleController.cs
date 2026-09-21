using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScrambleController : ControllerBase {
    [HttpGet("3x3")]
    public IActionResult GetScramble3x3() {
        var scrambleService = new ScrambleService();
        var scramble = scrambleService.GenerateScramble3x3();
        Console.WriteLine(scramble);
        return Ok(scramble);
    }

}
