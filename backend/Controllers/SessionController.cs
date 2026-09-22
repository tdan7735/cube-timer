using backend.Data;
using backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SessionController(AppDbContext context) : ControllerBase {
    [HttpGet]
    public async Task<IActionResult> GetSessions() {
        var user = await context.Users.FindAsync(1);
        if (user == null) {
            return NotFound("User not found");
        }

        var sessions = await context.Sessions.Where(s => s.UserId == user.Id).ToListAsync();

        if (sessions == null) {
            return NotFound("Session not found");
        }

        return Ok(sessions);
    }

    [HttpPost]
    public async Task<IActionResult> CreateSession([FromBody] CreateSessionRequest req) {
        var user = await context.Users.FindAsync(1);
        if (user == null) {
            return NotFound("User not found");
        }

        var session = new Session {
            Name = req.Name,
            Type = req.Type,
            UserId = user.Id,
            WhenMade = DateTime.UtcNow,
            Solves = [],
        };

        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        return Created(nameof(GetSessions), session);

    }
}

public class CreateSessionRequest {
    public required string Name { get; set; } = "";
    public required SessionType Type { get; set; }
}
