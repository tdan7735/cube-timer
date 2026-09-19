namespace Cube.Core;

public enum Corner {
    UFR, UFL, UBL, UBR,
    DFR, DFL, DBL, DBR
}

public enum Edge {
    UF, UR, UL, UB,
    DF, DR, DL, DB,
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

    // Corner indices
    private const int UFRIndex = 0;
    private const int UFLIndex = 1;
    private const int UBLIndex = 2;
    private const int UBRIndex = 3;
    private const int DFRIndex = 4;
    private const int DFLIndex = 5;
    private const int DBLIndex = 6;
    private const int DBRIndex = 7;

    // Edge indices
    private const int UFIndex = 0;
    private const int URIndex = 1;
    private const int ULIndex = 2;
    private const int UBIndex = 3;
    private const int DFIndex = 4;
    private const int DRIndex = 5;
    private const int DLIndex = 6;
    private const int DBIndex = 7;
    private const int FLIndex = 8;
    private const int FRIndex = 9;
    private const int BLIndex = 10;
    private const int BRIndex = 11;

    // Creates a new solved cube
    public Cube() {
        Corners = [
            Corner.UFR, Corner.UFL, Corner.UBL, Corner.UBR,
            Corner.DFR, Corner.DFL, Corner.DBL, Corner.DBR
        ];
        CornerOrientations = new int[8];

        Edges = [
            Edge.UF, Edge.UR, Edge.UL, Edge.UB,
            Edge.DF, Edge.DR, Edge.DL, Edge.DB,
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

            case Move.D:
                ApplyD();
                break;
            case Move.D2:
                ApplyD();
                ApplyD();
                break;
            case Move.DPrime:
                ApplyD();
                ApplyD();
                ApplyD();
                break;

            case Move.R:
                ApplyR();
                break;
            case Move.R2:
                ApplyR();
                ApplyR();
                break;
            case Move.RPrime:
                ApplyR();
                ApplyR();
                ApplyR();
                break;

            default:
                throw new NotImplementedException();
        }
    }

    /**
     * Returns the state of a cube as a string
    */
    public override string ToString() {
        return $"""
            Corners: {string.Join(", ", Corners)}
            CornerOrientations: {string.Join(", ", CornerOrientations)}
            Edges: {string.Join(", ", Edges)}
            EdgeOrientations: {string.Join(", ", EdgeOrientations)}
        """;
    }

    /**
     * Apply U Rotation to the cube
     * Corners:
     *     UFL -> UBL
     *     UBL -> UBR
     *     UBR -> UFR
     *     UFR -> UFL
     * Edges:
     *     UF -> UL
     *     UL -> UB
     *     UB -> UR
     *     UR -> UF
     * Corner and Edge Orientations are not affected
    */
    private void ApplyU() {
        (Corners[UFLIndex], Corners[UFRIndex], Corners[UBRIndex], Corners[UBLIndex])
            = (Corners[UBLIndex], Corners[UFLIndex], Corners[UFRIndex], Corners[UBRIndex]);

        (Edges[UFIndex], Edges[URIndex], Edges[UBIndex], Edges[ULIndex])
            = (Edges[ULIndex], Edges[UFIndex], Edges[URIndex], Edges[UBIndex]);
    }

    /**
     * Apply D Rotation to the cube
     * Corners:
     *     DFL -> DFR
     *     DFR -> DBR
     *     DBR -> DBL
     *     DBL -> DFL
     * Edges:
     *    DF -> DR
     *    DR -> DB
     *    DB -> DL
     *    DL -> DF
     */
    private void ApplyD() {
        (Corners[DFLIndex], Corners[DFRIndex], Corners[DBRIndex], Corners[DBLIndex])
            = (Corners[DFRIndex], Corners[DBRIndex], Corners[DBLIndex], Corners[DFLIndex]);

        (Edges[DFIndex], Edges[DRIndex], Edges[DBIndex], Edges[DLIndex])
            = (Edges[DRIndex], Edges[DBIndex], Edges[DLIndex], Edges[DFIndex]);
    }

    /**
     * Apply R Rotation to the cube
     * Corners:
     * Edges:
     */
    private void ApplyR() {
        // Moving edges and corners
        (Corners[UFRIndex], Corners[DFRIndex], Corners[DBRIndex], Corners[UBRIndex])
            = (Corners[UBRIndex], Corners[UFRIndex], Corners[DFRIndex], Corners[DBRIndex]);

        (Edges[FRIndex], Edges[DRIndex], Edges[BRIndex], Edges[URIndex])
            = (Edges[URIndex], Edges[FRIndex], Edges[DRIndex], Edges[BRIndex]);

        // Orienting edges and corners
    }
}
