-- Seeds the 57 standard OLL algorithms from oll-algs.pdf.
-- Run this after AlgorithmSeeder has created the OLL set and its cases.
-- The case labels below intentionally match AlgorithmSeeder exactly.

BEGIN;

WITH oll_algorithms(case_name, moves) AS (
    VALUES
        ('Dot Case', '(R U2 R'') (R'' F R F'') U2 (R'' F R F'')'),
        ('Dot Case', 'F (R U R'' U'') F'' f (R U R'' U'') f'''),
        ('Dot Case', 'y'' f (R U R'' U'') f'' (U'') F (R U R'' U'') F'''),
        ('Dot Case', 'y'' f (R U R'' U'') f'' (U) F (R U R'' U'') F'''),
        ('Square Shapes', 'r'' U2 (R U R'' U) r'),
        ('Square Shapes', 'r U2 (R'' U'' R U'') r'''),
        ('Lightning Shapes', 'r (U R'' U R) U2 r'''),
        ('Lightning Shapes', 'y2 r'' (U'' R U'' R'') U2 r'),
        ('Fish Shapes', 'y (R U R'' U'') (R'' F R) (R U R'' U'') F'''),
        ('Fish Shapes', '(R U R'' U) (R'' F R F'') (R U2 R'')'),
        ('Lightning Shapes', 'M (R U R'' U R U2 R'') U M'''),
        ('Lightning Shapes', 'y'' M'' (R'' U'' R U'' R'' U2 R) U'' M'),
        ('Knight Move Shapes', '(r U'' r'') U'' (r U r'') (F'' U F)'),
        ('Knight Move Shapes', 'R'' F (R U R'') F'' R (F U'' F'')'),
        ('Knight Move Shapes', '(r'' U'' r) (R'' U'' R U) (r'' U r)'),
        ('Knight Move Shapes', '(r U r'') (R U R'' U'') (r U'' r'')'),
        ('Dot Case', '(R U R'' U) (R'' F R F'') U2 (R'' F R F'')'),
        ('Dot Case', 'y (R U2 R'') (R'' F R F'') U2 M'' (U R U'' r'')'),
        ('Dot Case', 'M U (R U R'' U'') M'' (R'' F R F'')'),
        ('Dot Case', '(r U R'' U'') M2 (U R U'' R'') U'' M'''),
        ('OCLL', '(R U R'' U) (R U'' R'' U) (R U2 R'')'),
        ('OCLL', 'R U2 (R2'' U'') (R2 U'') (R2'' U'') U'' R'),
        ('OCLL', 'R2 D (R'' U2 R) D'' (R'' U2 R'')'),
        ('OCLL', '(r U R'' U'') (r'' F R F'')'),
        ('OCLL', 'y (F'' r U R'') (U'' r'' F R)'),
        ('OCLL', 'y R U2 (R'' U'' R U'') R'''),
        ('OCLL', '(R U R'' U) (R U2 R'')'),
        ('All Corners Orientated', '(r U R'' U'') M (U R U'' R'')'),
        ('Awkward Shapes', 'y (R U R'') U'' (R U'' R'') (F'' U'' F) (R U R'')'),
        ('Awkward Shapes', 'y2 F U (R U2 R'') U'' (R U2 R'') U'' F'''),
        ('P Shapes', '(R'' U'' F) (U R U'' R'') F'' R'),
        ('P Shapes', 'S (R U R'' U'') (R'' F R f'')'),
        ('T Shapes', '(R U R'' U'') (R'' F R F'')'),
        ('C Shapes', 'y2 R U R2 U'' R'' F (R U R U'') F'''),
        ('Fish Shapes', '(R U2 R'') (R'' F R F'') (R U2 R'')'),
        ('W Shapes', 'y2 (L'' U'' L U'') (L'' U L U) (L F'' L'' F)'),
        ('Fish Shapes', 'F R (U'' R'' U'') (R U R'') F'''),
        ('W Shapes', '(R U R'' U) (R U'' R'' U'') (R'' F R F'')'),
        ('Lightning Shapes', 'y L F'' (L'' U'' L U) F U'' L'''),
        ('Lightning Shapes', 'y R'' F (R U R'' U'') F'' U R'),
        ('Awkward Shapes', 'y2 (R U R'' U) (R U2 R'') F (R U R'' U'') F'''),
        ('Awkward Shapes', '(R'' U'' R U'') (R'' U2 R) F (R U R'' U'') F'''),
        ('P Shapes', 'y R'' U'' (F'' U F) R'),
        ('P Shapes', 'f (R U R'' U'') f'''),
        ('T Shapes', 'F (R U R'' U'') F'''),
        ('C Shapes', 'R'' U'' (R'' F R F'') U R'),
        ('L shapes', 'F'' (L'' U'' L U) (L'' U'' L U) F'),
        ('L shapes', 'F (R U R'' U'') (R U R'' U'') F'''),
        ('L shapes', 'y2 r U'' (r2 U) (r2 U) (r2) U'' r'),
        ('L shapes', 'r'' U (r2 U'') (r2 U'') (r2) U r'''),
        ('Line Shapes', 'f (R U R'' U'') (R U R'' U'') f'''),
        ('Line Shapes', 'y2 R'' (F'' U'' F U'') (R U R'' U) R'),
        ('L shapes', '(r'' U'' R U'') (R'' U R U'') (R'' U2 r)'),
        ('L shapes', '(r U R'' U) (R U'' R'' U) (R U2 r'')'),
        ('Line Shapes', 'R U2 R2 (U'' R U'' R'') U2 (F R F'')'),
        ('Line Shapes', '(r U r'') (U R U'' R'') (U R U'' R'') (r U'' r'')'),
        ('All Corners Orientated', '(R U R'' U'') M'' (U R U'' r'')')
)
INSERT INTO "Algorithms" ("Moves", "AlgorithmCaseId", "UserId")
SELECT oll_algorithms.moves, algorithm_cases."Id", NULL
FROM oll_algorithms
JOIN "AlgorithmCases" AS algorithm_cases
    ON algorithm_cases."Name" = oll_algorithms.case_name
JOIN "AlgorithmSets" AS algorithm_sets
    ON algorithm_sets."Id" = algorithm_cases."AlgorithmSetId"
   AND algorithm_sets."Name" = 'OLL'
WHERE NOT EXISTS (
    SELECT 1
    FROM "Algorithms" AS existing_algorithm
    WHERE existing_algorithm."AlgorithmCaseId" = algorithm_cases."Id"
      AND existing_algorithm."Moves" = oll_algorithms.moves
);

COMMIT;
