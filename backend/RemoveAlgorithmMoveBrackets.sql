-- Removes grouping parentheses from every stored algorithm and keeps move spacing tidy.
-- Example: "(R U R' U') F'" becomes "R U R' U' F'".

BEGIN;

UPDATE "Algorithms"
SET "Moves" = btrim(
    regexp_replace(
        regexp_replace("Moves", '[()]', '', 'g'),
        '\s+',
        ' ',
        'g'
    )
)
WHERE "Moves" ~ '[()]';

COMMIT;
