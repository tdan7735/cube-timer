using backend.Services;
using Microsoft.AspNetCore.Mvc;

namespace backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AlgorithmController(AlgorithmService service) : ControllerBase {
    [HttpGet]
    public async Task<ActionResult<List<AlgorithmSetResponse>>> GetAlgorithmSets() {
        var algorithmSets = await service.GetAlgorithmSets();
        var res = new List<AlgorithmSetResponse>();

        foreach (var set in algorithmSets) {
            var resSet = new AlgorithmSetResponse {
                Id = set.Id,
                Name = set.Name,
                Groups = [],
                Cases = [],
            };

            foreach (var group in set.Groups) {
                resSet.Groups.Add(new AlgorithmGroupResponse {
                    Id = group.Id,
                    Name = group.Name,
                    Cases = group.Cases.Select(ToCaseResponse).ToList(),
                });
            }

            resSet.Cases.AddRange(set.Cases
                .Where(c => c.AlgorithmGroupId == null)
                .Select(ToCaseResponse));
            res.Add(resSet);
        }

        return Ok(res);
    }

    [HttpGet("{name}")]
    public async Task<ActionResult<AlgorithmSetResponse>> GetAlgorithmSet([FromRoute] string name) {
        var algorithmSet = await service.GetAlgorithmSet(name);

        if (algorithmSet == null) {
            return NotFound();
        }

        var res = new AlgorithmSetResponse {
            Id = algorithmSet.Id,
            Name = algorithmSet.Name,
            Groups = [],
            Cases = [],
        };

        foreach (var group in algorithmSet.Groups) {
            res.Groups.Add(new AlgorithmGroupResponse {
                Id = group.Id,
                Name = group.Name,
                Cases = group.Cases.Select(ToCaseResponse).ToList(),
            });
        }

        res.Cases.AddRange(algorithmSet.Cases
            .Where(c => c.AlgorithmGroupId == null)
            .Select(ToCaseResponse));

        return Ok(res);
    }

    [HttpPut("{id:int}/standard")]
    public async Task<ActionResult<AlgorithmResponse>> SetStandardAlgorithm(int id) {
        var algorithm = await service.SetStandardAlgorithm(id);
        if (algorithm == null) return NotFound();
        return Ok(new AlgorithmResponse {
            Id = algorithm.Id,
            Moves = algorithm.Moves,
            UserId = algorithm.UserId,
            IsStandard = algorithm.IsStandard,
        });
    }

    private static AlgorithmCaseResponse ToCaseResponse(Models.AlgorithmCase algorithmCase) => new() {
        Id = algorithmCase.Id,
        Name = algorithmCase.Name,
        CaseNumber = algorithmCase.CaseNumber,
        Algorithms = algorithmCase.Algorithms.OrderBy(a => a.Id).Select(a => new AlgorithmResponse {
            Id = a.Id,
            Moves = a.Moves,
            UserId = a.UserId,
            IsStandard = a.IsStandard,
        }).ToList(),
    };
}

public class AlgorithmSetResponse {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required List<AlgorithmGroupResponse> Groups { get; set; }
    public required List<AlgorithmCaseResponse> Cases { get; set; }
}

public class AlgorithmGroupResponse {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required List<AlgorithmCaseResponse> Cases { get; set; }
}

public class AlgorithmCaseResponse {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public int? CaseNumber { get; set; }
    public required List<AlgorithmResponse> Algorithms { get; set; }
}

public class AlgorithmResponse {
    public bool IsStandard { get; set; }
    public required int Id { get; set; }
    public required string Moves { get; set; }
    public int? UserId { get; set; }
}
