using Microsoft.AspNetCore.Mvc;


namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SolvesController : ControllerBase {

    [HttpGet]
    public IActionResult GetSolves() {
        return Ok(new[]
                {
                    "12.42",
                    "11.83",
                    "67",
                });
    }
}
