namespace backend.Models;

public enum SessionType {
    Solve,
    PLLTraining,
    OLLTraining,
}

public class Session {
    public int Id { get; set; }
    public SessionType Type { get; set; }
    public DateTime? WhenMade { get; set; }
    public List<Solve> Solves { get; set; } = [];
    public int UserId { get; set; }
}
