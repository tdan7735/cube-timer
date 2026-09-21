using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScrambleController(ScrambleService scrambleService) : ControllerBase {
    [HttpGet("3x3")]
    public IActionResult GetScramble3x3() {
        var scramble = scrambleService.GenerateScramble3x3();
        return Ok(new { scramble });
    }

}
