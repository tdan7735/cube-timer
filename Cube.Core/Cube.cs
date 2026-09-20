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
    private const int UFLPosition = 0;
    private const int UBLPosition = 1;
    private const int UBRPosition = 2;
    private const int UFRPosition = 3;
    private const int DFLPosition = 4;
    private const int DBLPosition = 5;
    private const int DBRPosition = 6;
    private const int DFRPosition = 7;

    // Edge indices
    private const int UFPosition = 0;
    private const int ULPosition = 1;
    private const int UBPosition = 2;
    private const int URPosition = 3;
    private const int FLPosition = 4;
    private const int BLPosition = 5;
    private const int BRPosition = 6;
    private const int FRPosition = 7;
    private const int DFPosition = 8;
    private const int DLPosition = 9;
    private const int DBPosition = 10;
    private const int DRPosition = 11;

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
     * Corner orientations:
     *     If orietnation is 0, then it stays 0
     *     If orientation is 1, then it goes to 2
     *     If orientation is 2, then it goes to 1
     * Edge orientations stay the same since the upper edge will still be facing up
    */
    private void ApplyU() {
        (Corners[UFLPosition], Corners[UBLPosition], Corners[UBRPosition], Corners[UFRPosition])
            = (Corners[UFRPosition], Corners[UFLPosition], Corners[UBLPosition], Corners[UBRPosition]);

        (Edges[ULPosition], Edges[UBPosition], Edges[URPosition], Edges[UFPosition])
            = (Edges[UFPosition], Edges[ULPosition], Edges[UBPosition], Edges[URPosition]);

        (CornerOrientation[UFLPosition], CornerOrientation[UBLPosition], CornerOrientation[UBRPosition], CornerOrientation[UFRPosition])
            = (
                RotateCornerForTopOrBottomMove(CornerOrientation[UFRPosition]),
                RotateCornerForTopOrBottomMove(CornerOrientation[UFLPosition]),
                RotateCornerForTopOrBottomMove(CornerOrientation[UBLPosition]),
                RotateCornerForTopOrBottomMove(CornerOrientation[UBRPosition])
            );
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
        (Corners[DFRPosition], Corners[DBRPosition], Corners[DBLPosition], Corners[DFLPosition])
            = (Corners[DFLPosition], Corners[DFRPosition], Corners[DBRPosition], Corners[DBLPosition]);

        (Edges[DRPosition], Edges[DBPosition], Edges[DLPosition], Edges[DFPosition])
            = (Edges[DFPosition], Edges[DRPosition], Edges[DBPosition], Edges[DLPosition]);

        (CornerOrientation[DFRPosition], CornerOrientation[DBRPosition], CornerOrientation[DBLPosition], CornerOrientation[DFLPosition])
            = (
                RotateCornerForTopOrBottomMove(CornerOrientation[DFLPosition]),
                RotateCornerForTopOrBottomMove(CornerOrientation[DFRPosition]),
                RotateCornerForTopOrBottomMove(CornerOrientation[DBRPosition]),
                RotateCornerForTopOrBottomMove(CornerOrientation[DBLPosition])
            );
    }

    /**
     * Apply R Rotation to the cube
     * Corners:
     * Edges:
     */
    private void ApplyR() {
        // Moving edges and corners
        (Corners[UBRPosition], Corners[DBRPosition], Corners[DFRPosition], Corners[UFRPosition])
            = (Corners[UFRPosition], Corners[UBRPosition], Corners[DBRPosition], Corners[DFRPosition]);

        (Edges[URPosition], Edges[BRPosition], Edges[DRPosition], Edges[FRPosition])
            = (Edges[FRPosition], Edges[URPosition], Edges[BRPosition], Edges[DRPosition]);

        (CornerOrientation[UBRPosition], CornerOrientation[DBRPosition], CornerOrientation[DFRPosition], CornerOrientation[UFRPosition])
            = (
                RotateCornerForLeftOrRightMove(CornerOrientation[UFRPosition]),
                RotateCornerForLeftOrRightMove(CornerOrientation[UBRPosition]),
                RotateCornerForLeftOrRightMove(CornerOrientation[DBRPosition]),
                RotateCornerForLeftOrRightMove(CornerOrientation[DFRPosition])
              );
    }

    private void ApplyL() {
        (Corners[UFLPosition], Corners[DFLPosition], Corners[DBLPosition], Corners[UBLPosition]) =
            (Corners[UBLPosition], Corners[UFLPosition], Corners[DFLPosition], Corners[DBLPosition]);

        (Edges[ULPosition], Edges[FLPosition], Edges[DLPosition], Edges[BLPosition]) =
            (Edges[BLPosition], Edges[ULPosition], Edges[FLPosition], Edges[DLPosition]);

        (CornerOrientation[UFLPosition], CornerOrientation[DFLPosition], CornerOrientation[DBLPosition], CornerOrientation[UBLPosition])
            = (
                RotateCornerForFrontOrBackMove(CornerOrientation[UBLPosition]),
                RotateCornerForFrontOrBackMove(CornerOrientation[UFLPosition]),
                RotateCornerForFrontOrBackMove(CornerOrientation[DFLPosition]),
                RotateCornerForFrontOrBackMove(CornerOrientation[DBLPosition])
              );

    }

    private static int RotateCornerForTopOrBottomMove(int orientation) {
        return orientation switch {
            0 => 0,
            1 => 2,
            2 => 1,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation))
        };
    }

    private static int RotateCornerForLeftOrRightMove(int orientation) {
        return orientation switch {
            0 => 1,
            1 => 0,
            2 => 2,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation))
        };
    }

    private static int RotateCornerForFrontOrBackMove(int orientation) {
        return orientation switch {
            0 => 2,
            1 => 1,
            2 => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation))
        };
    }

    private static int RotateEdgeForFrontOrBackMove(int orientation) {
        return orientation switch {
            0 => 1,
            1 => 0,
            _ => throw new ArgumentOutOfRangeException(nameof(orientation))
        };
    }
}
