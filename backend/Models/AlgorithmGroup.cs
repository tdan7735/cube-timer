namespace backend.Models;

/// <summary>
/// An optional way to organise cases within an algorithm set.
/// OLL uses groups such as "Dot Case"; PLL remains flat.
/// </summary>
public class AlgorithmGroup {
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int AlgorithmSetId { get; set; }
    public AlgorithmSet AlgorithmSet { get; set; } = null!;

    public ICollection<AlgorithmCase> Cases { get; set; } = [];
}
