#!/usr/bin/env bash
set -euo pipefail

# Copies application data from Supabase PostgreSQL into the local database.
# Solves are deliberately excluded. Run this only against a newly migrated,
# otherwise empty local database.

: "${SOURCE_DATABASE_URL:?Set SOURCE_DATABASE_URL to the current Supabase PostgreSQL connection string.}"

local_user="${LOCAL_POSTGRES_USER:-cube_timer}"
local_database="${LOCAL_POSTGRES_DB:-cube_timer}"
export SOURCE_DATABASE_URL

existing_rows="$(docker compose exec -T database psql -At -U "$local_user" -d "$local_database" -c '
  SELECT
    (SELECT COUNT(*) FROM "Users") +
    (SELECT COUNT(*) FROM "Sessions") +
    (SELECT COUNT(*) FROM "AlgorithmSets") +
    (SELECT COUNT(*) FROM "AlgorithmGroups") +
    (SELECT COUNT(*) FROM "AlgorithmCases") +
    (SELECT COUNT(*) FROM "Algorithms");
')"

if [ "$existing_rows" != "0" ]; then
  echo "The local database already contains application data. Refusing to merge or overwrite it." >&2
  echo "Start with a fresh local database before importing." >&2
  exit 1
fi

docker compose exec -T -e SOURCE_DATABASE_URL database sh -c '
  pg_dump --data-only --inserts --no-owner --no-privileges \
    --table=public.\"Users\" \
    --table=public.\"AlgorithmSets\" \
    --table=public.\"AlgorithmGroups\" \
    --table=public.\"AlgorithmCases\" \
    --table=public.\"Algorithms\" \
    --table=public.\"Sessions\" \
    "$SOURCE_DATABASE_URL"
' | docker compose exec -T database psql -v ON_ERROR_STOP=1 -U "$local_user" -d "$local_database"

docker compose exec -T database psql -v ON_ERROR_STOP=1 -U "$local_user" -d "$local_database" <<'SQL'
SELECT setval(pg_get_serial_sequence('"Users"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Users"), 1));
SELECT setval(pg_get_serial_sequence('"AlgorithmSets"', 'Id'), COALESCE((SELECT MAX("Id") FROM "AlgorithmSets"), 1));
SELECT setval(pg_get_serial_sequence('"AlgorithmGroups"', 'Id'), COALESCE((SELECT MAX("Id") FROM "AlgorithmGroups"), 1));
SELECT setval(pg_get_serial_sequence('"AlgorithmCases"', 'Id'), COALESCE((SELECT MAX("Id") FROM "AlgorithmCases"), 1));
SELECT setval(pg_get_serial_sequence('"Algorithms"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Algorithms"), 1));
SELECT setval(pg_get_serial_sequence('"Sessions"', 'Id'), COALESCE((SELECT MAX("Id") FROM "Sessions"), 1));
SQL

echo "Imported users, sessions, algorithm data, and algorithm choices. Solves were not copied."
