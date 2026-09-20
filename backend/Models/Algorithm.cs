namespace backend.Models;

public class Algorithm {
    public int Id { get; set; }

    public string Moves { get; set; } = string.Empty;

    public int AlgorithmCaseId { get; set; }
    public AlgorithmCase AlgorithmCase { get; set; } = null!;

    public int? UserId { get; set; }
}
