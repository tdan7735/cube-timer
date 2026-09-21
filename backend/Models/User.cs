namespace backend.Models;

public class User {
    public int Id { get; set; }
    public string Username { get; set; } = string.Empty;
    public List<Session> Sessions { get; set; } = [];
}
