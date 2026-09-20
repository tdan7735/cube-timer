namespace backend.Models;

public class AlgorithmSet {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<AlgorithmCase> Cases { get; set; } = [];
}
