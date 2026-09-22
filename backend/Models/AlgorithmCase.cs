namespace backend.Models;

public class AlgorithmCase {
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    // OLL uses this to distinguish, for example, OLL 28 from OLL 57.
    // It remains null for named PLL cases such as Aa and T.
    public int? CaseNumber { get; set; }

    public int AlgorithmSetId { get; set; }
    public AlgorithmSet AlgorithmSet { get; set; } = null!;

    // Null for flat sets such as PLL.
    public int? AlgorithmGroupId { get; set; }
    public AlgorithmGroup? AlgorithmGroup { get; set; }

    public ICollection<Algorithm> Algorithms { get; set; } = [];
    public ICollection<Solve> Solves { get; set; } = [];
}
