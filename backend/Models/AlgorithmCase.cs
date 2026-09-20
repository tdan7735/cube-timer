namespace backend.Models;

public class AlgorithmCase {
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public int AlgorithmSetId { get; set; }
    public AlgorithmSet AlgorithmSet { get; set; } = null!;

    public ICollection<Algorithm> Algorithms { get; set; } = [];
}
