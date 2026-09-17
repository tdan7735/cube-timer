namespace backend.Services;

public class ScrambleService {
    private static readonly string[] Faces = ["R", "L", "U", "D", "F", "B"];
    private static readonly string[] Suffixes = ["", "'", "2"];

    public string GenerateScramble3x3(int length = 25) {
        var random = new Random();
        var moves = new List<string>();

        string? previousFace = null;
        
        while (moves.Count < length) {
            var face = Faces[random.Next(Faces.Length)];

            // prevents moving the same face twice in a row
            if (face == previousFace) {
                continue;
            }

            var suffix = Suffixes[random.Next(Suffixes.Length)];
            moves.Add($"{face}{suffix}");
            previousFace = face;
        }

        return "";
    }
}
