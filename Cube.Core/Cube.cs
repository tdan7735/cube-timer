namespace Cube.Core;

public enum Corner {
    UFL, UBL, UBR, UFR,
    DFL, DBL, DBR, DFR
}

public enum Edge {
    UF, UL, UB, UR,
    FL, BL, BR, FR,
    DF, DL, DB, DR,
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
    public int[] CornerOrientation { get; }

    public Edge[] Edges { get; }
    public int[] EdgeOrientation { get; }

    // Corner indices
    private const int UFLIndex = 0;
    private const int UBLIndex = 1;
    private const int UBRIndex = 2;
    private const int UFRIndex = 3;
    private const int DFLIndex = 4;
    private const int DBLIndex = 5;
    private const int DBRIndex = 6;
    private const int DFRIndex = 7;

    // Edge indices
    private const int UFIndex = 0;
    private const int ULIndex = 1;
    private const int UBIndex = 2;
    private const int URIndex = 3;
    private const int FLIndex = 4;
    private const int BLIndex = 5;
    private const int BRIndex = 6;
    private const int FRIndex = 7;
    private const int DFIndex = 8;
    private const int DLIndex = 9;
    private const int DBIndex = 10;
    private const int DRIndex = 11;

    // Creates a new solved cube
    public Cube() {
        Corners = [
            Corner.UFL, Corner.UBL, Corner.UBR, Corner.UFR,
            Corner.DFL, Corner.DBL, Corner.DBR, Corner.DFR
        ];
        CornerOrientation = new int[8];

        Edges = [
            Edge.UF, Edge.UL, Edge.UB, Edge.UR,
            Edge.FL, Edge.BL, Edge.BR, Edge.FR,
            Edge.DF, Edge.DL, Edge.DB, Edge.DR,
        ];
        EdgeOrientation = new int[12];
    }

    public bool IsSolved() {
        for (int i = 0; i < Corners.Length; i++) {
            if ((int)Corners[i] != i || CornerOrientation[i] != 0) return false;
        }

        for (int i = 0; i < Edges.Length; i++) {
            if ((int)Edges[i] != i || EdgeOrientation[i] != 0) return false;
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

            case Move.L:
                ApplyL();
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
            CornerOrientations: {string.Join(", ", CornerOrientation)}
            Edges: {string.Join(", ", Edges)}
            EdgeOrientations: {string.Join(", ", EdgeOrientation)}
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
    */
    private void ApplyU() {
        (Corners[UFLIndex], Corners[UBLIndex], Corners[UBRIndex], Corners[UFRIndex])
            = (Corners[UFRIndex], Corners[UFLIndex], Corners[UBLIndex], Corners[UBRIndex]);

        (Edges[ULIndex], Edges[UBIndex], Edges[URIndex], Edges[UFIndex])
            = (Edges[UFIndex], Edges[ULIndex], Edges[UBIndex], Edges[URIndex]);
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
        (Corners[DFRIndex], Corners[DBRIndex], Corners[DBLIndex], Corners[DFLIndex])
            = (Corners[DFLIndex], Corners[DFRIndex], Corners[DBRIndex], Corners[DBLIndex]);

        (Edges[DRIndex], Edges[DBIndex], Edges[DLIndex], Edges[DFIndex])
            = (Edges[DFIndex], Edges[DRIndex], Edges[DBIndex], Edges[DLIndex]);
    }

    /**
     * Apply R Rotation to the cube
     * Corners:
     * Edges:
     */
    private void ApplyR() {
        // Moving edges and corners
        (Corners[UBRIndex], Corners[DBRIndex], Corners[DFRIndex], Corners[UFRIndex])
            = (Corners[UFRIndex], Corners[UBRIndex], Corners[DBRIndex], Corners[DFRIndex]);

        (Edges[URIndex], Edges[BRIndex], Edges[DRIndex], Edges[FRIndex])
            = (Edges[FRIndex], Edges[URIndex], Edges[BRIndex], Edges[DRIndex]);

        // Orienting corners
        (CornerOrientation[UBRIndex], CornerOrientation[DBRIndex], CornerOrientation[DFRIndex], CornerOrientation[UFRIndex])
            = (
                TwistCorner(CornerOrientation[UFRIndex], 1),
                TwistCorner(CornerOrientation[UBRIndex], 2),
                TwistCorner(CornerOrientation[DBRIndex], 1),
                TwistCorner(CornerOrientation[DFRIndex], 2)
            );
    }

    private void ApplyL() {
        (Corners[UFLIndex], Corners[DFLIndex], Corners[DBLIndex], Corners[UBLIndex]) =
            (Corners[UBLIndex], Corners[UFLIndex], Corners[DFLIndex], Corners[DBLIndex]);

        (Edges[ULIndex], Edges[FLIndex], Edges[DLIndex], Edges[BLIndex]) =
            (Edges[BLIndex], Edges[ULIndex], Edges[FLIndex], Edges[DLIndex]);

        (CornerOrientation[UFLIndex], CornerOrientation[DFLIndex], CornerOrientation[DBLIndex], CornerOrientation[UBLIndex])
            = (
                TwistCorner(CornerOrientation[UBLIndex], 1),
                TwistCorner(CornerOrientation[UFLIndex], 2),
                TwistCorner(CornerOrientation[DFLIndex], 1),
                TwistCorner(CornerOrientation[DBLIndex], 2)
            );
    }

    private static int TwistCorner(int orientation, int delta) {
        return (orientation + delta) % 3;
    }
}
