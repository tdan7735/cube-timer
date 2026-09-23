namespace backend.Services;

public class ScrambleService {
    private static readonly string[] Faces = ["R", "U", "F", "L", "D", "B"];
    private static readonly string[] Suffixes = ["", "'", "2"];

    public string GenerateScramble3x3(int length = 20) {
        var random = new Random();
        var moves = new List<string>();

        string previousFace = "";

        while (moves.Count < length) {
            var face = Faces[random.Next(Faces.Length)];

            // prevents moving the same face twice in a row or moving the opposite face (e.g. R and L)
            if (face == previousFace) {
                continue;
            }
            if (IsOpposite(face, previousFace)) {
                continue;
            }


            var suffix = Suffixes[random.Next(Suffixes.Length)];
            moves.Add($"{face}{suffix}");
            previousFace = face;
        }

        return moves.Aggregate((a, b) => $"{a} {b}");
    }

    private static bool IsOpposite(string face1, string face2) {
        return Faces.IndexOf(face1) == Faces.IndexOf(face2) - (Faces.Length / 2)
            || Faces.IndexOf(face1) == Faces.IndexOf(face2) + (Faces.Length / 2);
    }
}
