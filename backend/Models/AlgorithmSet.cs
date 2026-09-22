namespace backend.Models;

public class AlgorithmSet {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;

    // Used by flat sets such as PLL.
    public ICollection<AlgorithmCase> Cases { get; set; } = [];

    // Used by grouped sets such as OLL.
    public ICollection<AlgorithmGroup> Groups { get; set; } = [];
    public ICollection<Session> TrainingSessions { get; set; } = [];
}
