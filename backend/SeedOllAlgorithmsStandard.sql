-- Seeds the 57 standard OLL algorithms from oll-algs.pdf.
-- Run this after AlgorithmSeeder has created the OLL groups and numbered cases.

BEGIN;

WITH oll_algorithms(case_number, moves) AS (
    VALUES
        (1, '(R U2 R'') (R'' F R F'') U2 (R'' F R F'')'),
        (2, 'F (R U R'' U'') F'' f (R U R'' U'') f'''),
        (3, 'y'' f (R U R'' U'') f'' (U'') F (R U R'' U'') F'''),
        (4, 'y'' f (R U R'' U'') f'' (U) F (R U R'' U'') F'''),
        (5, 'r'' U2 (R U R'' U) r'),
        (6, 'r U2 (R'' U'' R U'') r'''),
        (7, 'r (U R'' U R) U2 r'''),
        (8, 'y2 r'' (U'' R U'' R'') U2 r'),
        (9, 'y (R U R'' U'') (R'' F R) (R U R'' U'') F'''),
        (10, '(R U R'' U) (R'' F R F'') (R U2 R'')'),
        (11, 'M (R U R'' U R U2 R'') U M'''),
        (12, 'y'' M'' (R'' U'' R U'' R'' U2 R) U'' M'),
        (13, '(r U'' r'') U'' (r U r'') (F'' U F)'),
        (14, 'R'' F (R U R'') F'' R (F U'' F'')'),
        (15, '(r'' U'' r) (R'' U'' R U) (r'' U r)'),
        (16, '(r U r'') (R U R'' U'') (r U'' r'')'),
        (17, '(R U R'' U) (R'' F R F'') U2 (R'' F R F'')'),
        (18, 'y (R U2 R'') (R'' F R F'') U2 M'' (U R U'' r'')'),
        (19, 'M U (R U R'' U'') M'' (R'' F R F'')'),
        (20, '(r U R'' U'') M2 (U R U'' R'') U'' M'''),
        (21, '(R U R'' U) (R U'' R'' U) (R U2 R'')'),
        (22, 'R U2 (R2'' U'') (R2 U'') (R2'' U'') U'' R'),
        (23, 'R2 D (R'' U2 R) D'' (R'' U2 R'')'),
        (24, '(r U R'' U'') (r'' F R F'')'),
        (25, 'y (F'' r U R'') (U'' r'' F R)'),
        (26, 'y R U2 (R'' U'' R U'') R'''),
        (27, '(R U R'' U) (R U2 R'')'),
        (28, '(r U R'' U'') M (U R U'' R'')'),
        (29, 'y (R U R'') U'' (R U'' R'') (F'' U'' F) (R U R'')'),
        (30, 'y2 F U (R U2 R'') U'' (R U2 R'') U'' F'''),
        (31, '(R'' U'' F) (U R U'' R'') F'' R'),
        (32, 'S (R U R'' U'') (R'' F R f'')'),
        (33, '(R U R'' U'') (R'' F R F'')'),
        (34, 'y2 R U R2 U'' R'' F (R U R U'') F'''),
        (35, '(R U2 R'') (R'' F R F'') (R U2 R'')'),
        (36, 'y2 (L'' U'' L U'') (L'' U L U) (L F'' L'' F)'),
        (37, 'F R (U'' R'' U'') (R U R'') F'''),
        (38, '(R U R'' U) (R U'' R'' U'') (R'' F R F'')'),
        (39, 'y L F'' (L'' U'' L U) F U'' L'''),
        (40, 'y R'' F (R U R'' U'') F'' U R'),
        (41, 'y2 (R U R'' U) (R U2 R'') F (R U R'' U'') F'''),
        (42, '(R'' U'' R U'') (R'' U2 R) F (R U R'' U'') F'''),
        (43, 'y R'' U'' (F'' U F) R'),
        (44, 'f (R U R'' U'') f'''),
        (45, 'F (R U R'' U'') F'''),
        (46, 'R'' U'' (R'' F R F'') U R'),
        (47, 'F'' (L'' U'' L U) (L'' U'' L U) F'),
        (48, 'F (R U R'' U'') (R U R'' U'') F'''),
        (49, 'y2 r U'' (r2 U) (r2 U) (r2) U'' r'),
        (50, 'r'' U (r2 U'') (r2 U'') (r2) U r'''),
        (51, 'f (R U R'' U'') (R U R'' U'') f'''),
        (52, 'y2 R'' (F'' U'' F U'') (R U R'' U) R'),
        (53, '(r'' U'' R U'') (R'' U R U'') (R'' U2 r)'),
        (54, '(r U R'' U) (R U'' R'' U) (R U2 r'')'),
        (55, 'R U2 R2 (U'' R U'' R'') U2 (F R F'')'),
        (56, '(r U r'') (U R U'' R'') (U R U'' R'') (r U'' r'')'),
        (57, '(R U R'' U'') M'' (U R U'' r'')')
)
INSERT INTO "Algorithms" ("Moves", "AlgorithmCaseId", "UserId")
SELECT oll_algorithms.moves, algorithm_cases."Id", NULL
FROM oll_algorithms
JOIN "AlgorithmCases" AS algorithm_cases
    ON algorithm_cases."CaseNumber" = oll_algorithms.case_number
JOIN "AlgorithmSets" AS algorithm_sets
    ON algorithm_sets."Id" = algorithm_cases."AlgorithmSetId"
   AND algorithm_sets."Name" = 'OLL'
WHERE NOT EXISTS (
    SELECT 1 FROM "Algorithms" AS existing_algorithm
    WHERE existing_algorithm."AlgorithmCaseId" = algorithm_cases."Id"
      AND existing_algorithm."Moves" = oll_algorithms.moves
);

COMMIT;
