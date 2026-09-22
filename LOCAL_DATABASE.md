# Local database

The app can run against a local PostgreSQL 17 container instead of Supabase.
The local server listens on port `54329` to avoid clashing with a machine-wide
PostgreSQL installation.

## First-time setup

1. Enable Docker Desktop's WSL integration for the distribution that runs this project.
2. From the repository root, start PostgreSQL:

   ```bash
   docker compose up -d database
   ```

3. Apply the EF Core schema from the `backend` directory. This explicitly targets the local container rather than the Supabase connection stored in user secrets:

   ```bash
   Db__DefaultConnection='Host=localhost;Port=54329;Database=cube_timer;Username=cube_timer;Password=cube_timer_dev' dotnet ef database update
   ```

4. Import non-solve data from Supabase. The source URL is used only by this command and is not written to disk:

   ```bash
   export SOURCE_DATABASE_URL='your Supabase PostgreSQL connection string'
   ./scripts/migrate-supabase-data.sh
   ```

   This imports users, sessions, algorithm sets, groups, cases, and algorithms. It deliberately does **not** import the `Solves` table.

5. Run the backend with the local connection profile:

   ```bash
   dotnet run --launch-profile local
   ```

The normal frontend development command continues to proxy API calls to `http://localhost:5164`.

## Daily use

```bash
docker compose up -d database
cd backend && dotnet run --launch-profile local
```

To stop the database without deleting data:

```bash
docker compose stop database
```
