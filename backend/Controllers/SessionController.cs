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
        var user = await context.Users.Where(u => u.Username == UserSeeder.DefaultUsername).FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var sessions = await context.Sessions.Where(s => s.UserId == user.Id).ToListAsync();
        return Ok(sessions);
    }

    [HttpGet("{sessionId}")]
    public async Task<IActionResult> GetSession([FromRoute] int sessionId) {
        var user = await context.Users.Where(u => u.Username == UserSeeder.DefaultUsername).FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var sessions = await context.Sessions.Where(s => s.UserId == user.Id).ToListAsync();
        var session = sessions.FirstOrDefault(s => s.Id == sessionId);

        if (session == null) {
            return NotFound("Session not found");
        }

        return Ok(session);
    }

    [HttpPost]
    public async Task<IActionResult> PostSession([FromBody] PostSessionRequest req) {
        var user = await context.Users.Where(u => u.Username == UserSeeder.DefaultUsername).FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var session = new Session {
            Name = req.Name ?? string.Empty,
            Type = req.Type,
            UserId = user.Id,
            WhenMade = DateTime.UtcNow,
            Solves = [],
        };

        context.Sessions.Add(session);
        await context.SaveChangesAsync();

        if (string.IsNullOrWhiteSpace(req.Name)) {
            session.Name = session.Id.ToString();
            await context.SaveChangesAsync();
        }

        return Created(nameof(GetSessions), session);
    }

    [HttpPut]
    public async Task<IActionResult> UpdateSession([FromRoute] int sessionId, [FromBody] PostSessionRequest req) {
        var user = await context.Users.Where(u => u.Username == UserSeeder.DefaultUsername).FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var sessions = await context.Sessions.Where(s => s.UserId == user.Id).ToListAsync();
        var session = sessions.FirstOrDefault(s => s.Id == sessionId);

        if (sessions == null) {
            return NotFound("User does not own this session");
        }

        if (session == null) {
            return NotFound("Session not found");
        }

        session.Name = string.IsNullOrWhiteSpace(req.Name)
            ? session.Id.ToString()
            : req.Name;
        session.Type = req.Type;

        await context.SaveChangesAsync();

        return Ok(session);
    }

    [HttpDelete("{sessionId}")]
    public async Task<IActionResult> DeleteSession([FromRoute] int sessionId) {
        var user = await context.Users.Where(u => u.Username == UserSeeder.DefaultUsername).FirstOrDefaultAsync();
        if (user == null) {
            return NotFound("User not found");
        }

        var sessions = await context.Sessions.Where(s => s.UserId == user.Id).ToListAsync();
        var session = sessions.FirstOrDefault(s => s.Id == sessionId);

        if (sessions.Count == 0) {
            return NotFound("User does not own this session");
        }

        if (session == null) {
            return NotFound("Session not found");
        }

        // Delete all solves for the session first then delete the session
        context.Solves.RemoveRange(context.Solves.Where(s => s.SessionId == sessionId));
        context.Sessions.Remove(session);

        await context.SaveChangesAsync();

        return Ok();
    }
}

public class PostSessionRequest {
    public string? Name { get; set; }
    public required SessionType Type { get; set; }
}
