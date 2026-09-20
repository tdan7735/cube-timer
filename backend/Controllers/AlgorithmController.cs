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
                Cases = [],
            };

            foreach (var c in set.Cases) {
                var resCase = new AlgorithmCaseResponse {
                    Id = c.Id,
                    Name = c.Name,
                    Algorithms = [],
                };

                foreach (var a in c.Algorithms) {
                    var resAlgorithm = new AlgorithmResponse {
                        Id = a.Id,
                        Moves = a.Moves,
                        UserId = a.UserId,
                    };

                    resCase.Algorithms.Add(resAlgorithm);
                }

                resSet.Cases.Add(resCase);
            }
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
            Cases = [],
        };

        foreach (var c in algorithmSet.Cases) {
            var resCase = new AlgorithmCaseResponse {
                Id = c.Id,
                Name = c.Name,
                Algorithms = [],
            };

            foreach (var a in c.Algorithms) {
                var resAlgorithm = new AlgorithmResponse {
                    Id = a.Id,
                    Moves = a.Moves,
                    UserId = a.UserId,
                };

                resCase.Algorithms.Add(resAlgorithm);
            }

            res.Cases.Add(resCase);
        }


        return Ok(res);
    }
}

public class AlgorithmSetResponse {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required List<AlgorithmCaseResponse> Cases { get; set; }
}

public class AlgorithmCaseResponse {
    public required int Id { get; set; }
    public required string Name { get; set; }
    public required List<AlgorithmResponse> Algorithms { get; set; }
}

public class AlgorithmResponse {
    public required int Id { get; set; }
    public required string Moves { get; set; }
    public int? UserId { get; set; }
}
