namespace Cube.Core;

public enum Corner {
    UFR, UFL, UBL, UBR,
    DFR, DFL, DBL, DBR
}

public enum Edge {
    UF, UR, UL, UB,
    DF, DR, DB, DL,
    FL, FR, BL, BR,
}

public enum Move {
    U, U2, UPrime,
    R, R2, RPrime,
    D, D2, DPrime,
    L, L2, LPrime,
    F, F2, FPrime,
    B, B2, BPrime
}

public class Cube {
    public Corner[] Corners { get; }
    public int[] CornerOrientations { get; }

    public Edge[] Edges { get; }
    public int[] EdgeOrientations { get; }

    // Creates a new cube with all its faces and edges set to 0 (solved)
    public Cube() {
        Corners = [
            Corner.UFR, Corner.UFL, Corner.UBL, Corner.UBR,
            Corner.DFR, Corner.DFL, Corner.DBL, Corner.DBR
        ];
        CornerOrientations = new int[8];

        Edges = [
            Edge.UF, Edge.UR, Edge.UL, Edge.UB,
            Edge.DF, Edge.DR, Edge.DB, Edge.DL,
            Edge.FL, Edge.FR, Edge.BL, Edge.BR
        ];
        EdgeOrientations = new int[12];
    }

    public bool IsSolved() {
        for (int i = 0; i < Corners.Length; i++) {
            if ((int)Corners[i] != i || CornerOrientations[i] != 0) return false;
        }

        for (int i = 0; i < Edges.Length; i++) {
            if ((int)Edges[i] != i || EdgeOrientations[i] != 0) return false;
        }

        return true;
    }

    public void ApplyMove(Move move) {
        switch (move) {
            case Move.U:
                ApplyU();
                break;
            case Move.U2:
                ApplyU();
                ApplyU();
                break;
            case Move.UPrime:
                ApplyU();
                ApplyU();
                ApplyU();
                break;

            default:
                throw new NotImplementedException();
        }
    }

    public override string ToString() {
        return $"""
            Corners: {string.Join(", ", Corners)}
            CornerOrientations: {string.Join(", ", CornerOrientations)}
            Edges: {string.Join(", ", Edges)}
            EdgeOrientations: {string.Join(", ", EdgeOrientations)}
        """;
    }

    private void ApplyU() {
        (Corners[0], Corners[1], Corners[2], Corners[3]) = (Corners[1], Corners[2], Corners[3], Corners[0]);

        (Edges[0], Edges[1], Edges[2], Edges[3]) = (Edges[1], Edges[2], Edges[3], Edges[0]);
    }
}
