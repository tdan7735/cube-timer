# Docker development environment

The frontend, backend, PostgreSQL database, and pgweb database browser can all
run in Docker. The database data is kept in the `cube_timer_postgres` volume.

## First-time setup

1. From the repository root, build and start the complete application:

   ```bash
   docker compose up --build
   ```

2. Open the application at [http://localhost:3000](http://localhost:3000).
   The backend is also exposed at [http://localhost:5164](http://localhost:5164),
   and pgweb is available at [http://localhost:8081](http://localhost:8081).

The backend automatically applies EF Core migrations and seeds required data
when it starts. No separate migration command is needed.

To run the stack in the background:

```bash
docker compose up --build -d
```

To stop it without deleting database data:

```bash
docker compose down
```

To also delete the local database data and start fresh next time:

```bash
docker compose down --volumes
```

## Configuration

Ports and database credentials can be overridden with environment variables:

- `FRONTEND_PORT` (default `3000`)
- `BACKEND_PORT` (default `5164`)
- `LOCAL_POSTGRES_PORT` (default `54329`)
- `LOCAL_PGWEB_PORT` (default `8081`)
- `LOCAL_POSTGRES_DB`, `LOCAL_POSTGRES_USER`, and `LOCAL_POSTGRES_PASSWORD`

For example: `FRONTEND_PORT=3001 docker compose up --build`.

## Optional Supabase data import

With the database container running, import non-solve data from Supabase using:

```bash
export SOURCE_DATABASE_URL='your Supabase PostgreSQL connection string'
./scripts/migrate-supabase-data.sh
```

This imports users, sessions, algorithm sets, groups, cases, and algorithms. It
deliberately does not import the `Solves` table.
