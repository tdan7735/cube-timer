namespace backend.Models;

public enum SessionType {
    Solves,
    AlgorithmTraining,
}

public class Session {
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public SessionType Type { get; set; }
    public DateTime WhenMade { get; set; }
    public List<Solve> Solves { get; set; } = [];
    public int UserId { get; set; }
    public User User { get; set; } = null!;
}
