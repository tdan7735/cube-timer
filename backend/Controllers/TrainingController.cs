using backend.Data;
using backend.Models;
using backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TrainingController(AppDbContext context, StatisticsService statistics) : ControllerBase {
    [HttpGet("{algorithmSetName}")]
    public async Task<IActionResult> GetTraining([FromRoute] string algorithmSetName) {
        var result = await GetTrainingData(algorithmSetName);
        return result.Error is not null ? result.Error : Ok(ToResponse(result.Session!, result.Attempts!));
    }

    [HttpGet("{algorithmSetName}/case-statistics")]
    public async Task<IActionResult> GetCaseStatistics([FromRoute] string algorithmSetName) {
        var user = await GetDefaultUser();
        if (user is null) return NotFound("User not found");

        var algorithmSet = await context.AlgorithmSets
            .SingleOrDefaultAsync(set => set.Name.ToLower() == algorithmSetName.ToLower());
        if (algorithmSet is null) return NotFound("Algorithm set not found");
        if (GetSolveType(algorithmSet) is null) {
            return BadRequest("Training is currently available only for OLL and PLL.");
        }

        var caseStatistics = await context.Solves
            .Where(solve =>
                solve.Session != null &&
                solve.Session.UserId == user.Id &&
                solve.Session.Type == SessionType.AlgorithmTraining &&
                solve.Session.AlgorithmSetId == algorithmSet.Id &&
                solve.AlgorithmCaseId != null)
            .GroupBy(solve => new { solve.AlgorithmCaseId, solve.AlgorithmCase!.Name })
            .Select(group => new CaseStatisticsResponse {
                AlgorithmCaseId = group.Key.AlgorithmCaseId!.Value,
                AlgorithmCaseName = group.Key.Name,
                AttemptCount = group.Count(),
                BestTime = group
                    .Where(solve => solve.Penalty != Penalty.DNF)
                    .Min(solve => (int?)(solve.SolveTime + (solve.Penalty == Penalty.Plus2 ? 2000 : 0))),
                AverageTime = group
                    .Where(solve => solve.Penalty != Penalty.DNF)
                    .Average(solve => (double?)(solve.SolveTime + (solve.Penalty == Penalty.Plus2 ? 2000 : 0))),
            })
            .ToListAsync();

        return Ok(caseStatistics);
    }

    [HttpPost("{algorithmSetName}/attempts")]
    public async Task<IActionResult> PostAttempt([FromRoute] string algorithmSetName, [FromBody] PostTrainingAttemptRequest request) {
        if (string.IsNullOrWhiteSpace(request.Scramble)) {
            return BadRequest("Scramble must not be empty.");
        }
        if (request.SolveTime < 0) {
            return BadRequest("Solve time cannot be negative.");
        }
        if (!Enum.IsDefined(request.Penalty)) {
            return BadRequest("Penalty is invalid.");
        }

        var user = await GetDefaultUser();
        if (user is null) return NotFound("User not found");

        var algorithmSet = await context.AlgorithmSets
            .SingleOrDefaultAsync(set => set.Name.ToLower() == algorithmSetName.ToLower());
        if (algorithmSet is null) return NotFound("Algorithm set not found");

        var solveType = GetSolveType(algorithmSet);
        if (solveType is null) return BadRequest("Training is currently available only for OLL and PLL.");

        var algorithmCase = await context.AlgorithmCases.SingleOrDefaultAsync(algorithmCase =>
            algorithmCase.Id == request.AlgorithmCaseId &&
            algorithmCase.AlgorithmSetId == algorithmSet.Id);
        if (algorithmCase is null) return BadRequest("Algorithm case does not belong to this training set.");

        var session = await GetOrCreateTrainingSession(user, algorithmSet);
        var attempt = new Solve {
            Scramble = request.Scramble,
            Penalty = request.Penalty,
            SolveTime = request.SolveTime,
            TimeSolved = DateTime.UtcNow,
            Type = solveType.Value,
            SessionId = session.Id,
            AlgorithmCaseId = algorithmCase.Id,
            AlgorithmCase = algorithmCase,
        };

        context.Solves.Add(attempt);
        await context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetTraining),
            new { algorithmSetName = algorithmSet.Name },
            SolveResponse.From(attempt));
    }

    [HttpDelete("{algorithmSetName}/attempts/{attemptId}")]
    public async Task<IActionResult> DeleteAttempt([FromRoute] string algorithmSetName, [FromRoute] int attemptId) {
        var result = await GetTrainingData(algorithmSetName);
        if (result.Error is not null) return result.Error;

        var attempt = result.Attempts!.SingleOrDefault(attempt => attempt.Id == attemptId);
        if (attempt is null) return NotFound("Training attempt not found");

        context.Solves.Remove(attempt);
        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{algorithmSetName}/attempts")]
    public async Task<IActionResult> DeleteAttempts([FromRoute] string algorithmSetName) {
        var result = await GetTrainingData(algorithmSetName);
        if (result.Error is not null) return result.Error;

        context.Solves.RemoveRange(result.Attempts!);
        await context.SaveChangesAsync();
        return NoContent();
    }

    private async Task<(User? User, AlgorithmSet? AlgorithmSet, Session? Session, List<Solve>? Attempts, IActionResult? Error)> GetTrainingData(string algorithmSetName) {
        var user = await GetDefaultUser();
        if (user is null) return (null, null, null, null, NotFound("User not found"));

        var algorithmSet = await context.AlgorithmSets
            .SingleOrDefaultAsync(set => set.Name.ToLower() == algorithmSetName.ToLower());
        if (algorithmSet is null) return (user, null, null, null, NotFound("Algorithm set not found"));
        if (GetSolveType(algorithmSet) is null) {
            return (user, algorithmSet, null, null, BadRequest("Training is currently available only for OLL and PLL."));
        }

        var session = await GetOrCreateTrainingSession(user, algorithmSet);
        var attempts = await context.Solves
            .Where(solve => solve.SessionId == session.Id)
            .Include(solve => solve.AlgorithmCase)
            .OrderByDescending(solve => solve.TimeSolved)
            .ToListAsync();

        return (user, algorithmSet, session, attempts, null);
    }

    private async Task<User?> GetDefaultUser() => await context.Users
        .SingleOrDefaultAsync(user => user.Username == UserSeeder.DefaultUsername);

    private async Task<Session> GetOrCreateTrainingSession(User user, AlgorithmSet algorithmSet) {
        var session = await context.Sessions.SingleOrDefaultAsync(session =>
            session.UserId == user.Id &&
            session.Type == SessionType.AlgorithmTraining &&
            session.AlgorithmSetId == algorithmSet.Id);
        if (session is not null) return session;

        session = new Session {
            Name = $"{algorithmSet.Name} Training",
            Type = SessionType.AlgorithmTraining,
            UserId = user.Id,
            AlgorithmSetId = algorithmSet.Id,
            WhenMade = DateTime.UtcNow,
        };
        context.Sessions.Add(session);
        await context.SaveChangesAsync();
        return session;
    }

    private static SolveType? GetSolveType(AlgorithmSet algorithmSet) => algorithmSet.Name.ToUpperInvariant() switch {
        "OLL" => SolveType.OLL,
        "PLL" => SolveType.PLL,
        _ => null,
    };

    private TrainingResponse ToResponse(Session session, List<Solve> attempts) => new() {
        Session = SessionResponse.From(session),
        Attempts = attempts.Select(SolveResponse.From).ToList(),
        Statistics = new TrainingStatisticsResponse {
            TotalAverage = statistics.CalculateTotalAverage(attempts),
            Ao5 = statistics.CalculateAo5(attempts),
            Ao12 = statistics.CalculateAo12(attempts),
            Ao50 = statistics.CalculateAo50(attempts),
            Ao100 = statistics.CalculateAo100(attempts),
            PersonalBest = statistics.GetPersonalBest(attempts),
        },
    };
}

public class PostTrainingAttemptRequest {
    public required string Scramble { get; set; }
    public required Penalty Penalty { get; set; }
    public required int SolveTime { get; set; }
    public required int AlgorithmCaseId { get; set; }
}

public class TrainingResponse {
    public required SessionResponse Session { get; set; }
    public required List<SolveResponse> Attempts { get; set; }
    public required TrainingStatisticsResponse Statistics { get; set; }
}

public class TrainingStatisticsResponse {
    public double? TotalAverage { get; set; }
    public double? Ao5 { get; set; }
    public double? Ao12 { get; set; }
    public double? Ao50 { get; set; }
    public double? Ao100 { get; set; }
    public double? PersonalBest { get; set; }
}

public class CaseStatisticsResponse {
    public int AlgorithmCaseId { get; set; }
    public required string AlgorithmCaseName { get; set; }
    public int AttemptCount { get; set; }
    public int? BestTime { get; set; }
    public double? AverageTime { get; set; }
}
