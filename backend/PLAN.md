# 🧩 Project: CubeLab

I'd frame the project as something like:

> **CubeLab — Rubik's Cube Training & Performance Platform**

The core idea is a personal platform where you can:

* record solves
* generate scrambles
* calculate averages
* analyse performance
* track algorithms
* identify weaknesses
* practise algorithms

The important part is that **version 1 should be small**. Don't start by trying to build the entire platform.

---

# 1. Technology stack

I'd recommend this stack:

### Backend

**C# + ASP.NET Core Web API**

This is the most important part for your job application.

You'll learn:

* controllers
* REST APIs
* dependency injection
* middleware
* authentication
* validation
* logging
* error handling

### ORM

**Entity Framework Core**

This gives you experience with:

* `DbContext`
* entities
* relationships
* migrations
* LINQ
* database queries

### Database

**SQL Server**

I'd specifically use SQL Server rather than PostgreSQL for this project because you're trying to demonstrate Microsoft/.NET skills.

You already know PostgreSQL, so this is a relatively easy transition.

### Frontend

I'd use:

**React + TypeScript**

rather than learning another frontend framework.

You already have React/Next.js experience, so the purpose of this project is to learn **.NET**, not relearn frontend development. 

You could use:

* React
* TypeScript
* Tailwind CSS
* Recharts

### Testing

**xUnit**

For:

* unit tests
* integration tests

You already have Jest/Vitest/Playwright experience, so learning xUnit gives you a nice way to demonstrate that your testing knowledge transfers to .NET. 

I'd also use:

**WebApplicationFactory**

for ASP.NET Core integration tests.

### Authentication

I'd start with:

**ASP.NET Core Identity + JWT**

Later, if you want to go further:

**Microsoft Entra ID**

But don't make authentication your first task.

### Documentation

**Swagger / OpenAPI**

ASP.NET Core can generate an API specification and interactive documentation.

This is an easy win for the portfolio.

### DevOps

Eventually:

* Docker
* GitHub Actions
* Azure

I'd deploy the backend to Azure if you get that far.

---

# 2. Overall architecture

I'd keep the architecture relatively simple:

```text
                    ┌─────────────────────┐
                    │   React Frontend    │
                    │    TypeScript       │
                    └──────────┬──────────┘
                               │
                          HTTP / REST
                               │
                               ▼
                    ┌─────────────────────┐
                    │ ASP.NET Core API    │
                    │       C#            │
                    └──────────┬──────────┘
                               │
                ┌──────────────┼──────────────┐
                ▼              ▼              ▼
           Solve Service   Cube Service   User Service
                │              │              │
                └──────────────┼──────────────┘
                               ▼
                    ┌─────────────────────┐
                    │    EF Core          │
                    └──────────┬──────────┘
                               ▼
                    ┌─────────────────────┐
                    │     SQL Server      │
                    └─────────────────────┘
```

Don't over-engineer it with microservices.

**One ASP.NET Core application is plenty.**

---

# 3. Start with the database

Before building the UI, think about what you actually need to store.

I'd start with something like:

```text
User
 └── Solve
      ├── Time
      ├── Scramble
      ├── Penalty
      ├── Date
      └── Session

User
 └── Session
      └── Solves

User
 └── AlgorithmProgress
      ├── Algorithm
      ├── RecognitionTime
      ├── ExecutionTime
      └── Accuracy
```

Eventually:

```text
User
Algorithm
AlgorithmCategory
Solve
Session
TrainingSession
TrainingAttempt
PersonalBest
```

Don't worry about getting the schema perfect immediately.

One of the things you'll learn from this project is **database design through iteration**.

---

# 4. Build Version 1: Solve Tracker

This should be your first milestone.

Forget authentication, Azure, fancy analytics, etc.

Just make this work:

### Add a solve

```text
Time:       12.42
Scramble:   R U R' F2...
Penalty:    None
```

Click:

**Save**

and it goes into SQL Server.

Then display:

```text
Today's Solves

12.42
11.83
13.21
10.92
12.11

Ao5: 12.12
Ao12: 12.38
PB: 10.92
```

This alone teaches you a huge amount.

---

# 5. Build your first API

For example:

```text
GET    /api/solves
GET    /api/solves/{id}
POST   /api/solves
PUT    /api/solves/{id}
DELETE /api/solves/{id}
```

Your `POST` might receive:

```json
{
  "time": 12.42,
  "scramble": "R U R' F2",
  "penalty": "None"
}
```

ASP.NET Core processes it:

```text
Controller
    ↓
DTO
    ↓
Validation
    ↓
Service
    ↓
EF Core
    ↓
SQL Server
```

This is the kind of architecture you'll want to understand very well for the .NET job.

---

# 6. Build the statistics engine

This is where the project becomes more interesting.

Implement:

### Personal best

```text
PB = minimum valid solve time
```

### Mean

```text
mean = sum(times) / number_of_solves
```

### Ao5

Take the most recent five solves, remove the highest and lowest, average the remaining three.

### Ao12

Same concept with twelve solves.

Then build:

* Ao5 history
* Ao12 history
* Ao100
* rolling averages
* best/worst sessions
* improvement over time

You could use a chart:

```text
Average
  │
14│ █
13│ ██
12│ ███
11│ ████
10│ █████
  └────────────
    Week 1 → Week 8
```

This gives you an opportunity to demonstrate **actual business logic**, rather than just database CRUD.

---

# 7. Add sessions

Instead of treating every solve independently:

```text
Session
 ├── Solve 1
 ├── Solve 2
 ├── Solve 3
 ├── ...
 └── Solve 20
```

A session could have:

* date
* cube
* event
* number of solves
* average
* PB
* notes

Then you can ask:

> "How did I perform during today's session?"

This makes the application much more useful.

---

# 8. Add algorithm training

This is where I think the project starts becoming **really interesting**.

Create an algorithm database.

For example:

```text
PLL

Aa Perm
Ab Perm
E Perm
F Perm
Ga Perm
Gb Perm
Gc Perm
Gd Perm
H Perm
Ja Perm
...
```

For each algorithm:

```text
Algorithm:
T Perm

Recognition:
[image]

Execution:
R U R' U'
R' F R2
U' R' U' R
U R' F'
```

Then let yourself record practice attempts.

For example:

```text
T Perm

Attempts:       47
Success Rate:   93.6%
Average:        1.24 sec
Best:           0.91 sec

Last practiced:
2 days ago
```

Now you have a **training system**, not just a timer.

---

# 9. Add authentication

Once the core application works:

```text
Register
Login
Logout
```

Then:

```text
GET /api/me/solves
```

only returns your own solves.

This gives you experience with:

* JWT
* authentication
* authorisation
* secure APIs
* user-specific data

Your existing capstone already has authentication and role-based access experience, so this is another opportunity to show that you can implement it in a completely different stack. 

---

# 10. Add proper testing

I'd aim for:

### Unit tests

Test:

```text
CalculateAo5()
CalculateAo12()
CalculateAverage()
GenerateScramble()
ValidateSolve()
CalculatePersonalBest()
```

### Integration tests

Test:

```text
POST /api/solves
GET /api/solves
DELETE /api/solves/{id}
```

including the database.

### Frontend

You could continue using Playwright, which you already know. 

Then you can genuinely put:

> **xUnit, integration testing, Playwright**

on your resume.

---

# 11. Add production-quality features

Once the actual product works, this is where you make it impressive to a recruiter.

### Logging

Use:

**Serilog**

or Microsoft's built-in logging abstractions.

Log things like:

```text
Solve created
User authenticated
Failed database operation
Invalid solve submitted
```

### Global error handling

Don't return ugly stack traces.

Have a consistent API error response.

### Validation

Use:

**FluentValidation**

or ASP.NET Core's built-in validation mechanisms.

### Health checks

Create:

```text
GET /health
```

that verifies the application and database are functioning.

### Swagger

Document every API endpoint.

---

# 12. Dockerise it

Your current resume already lists Docker. 

Use Docker to run:

```text
React
ASP.NET Core
SQL Server
```

You could eventually use Docker Compose:

```text
docker compose up
```

and your entire application launches.

That's a nice demonstration of practical engineering.

---

# 13. Add CI/CD

Use GitHub Actions.

Every pull request:

```text
        Pull Request
              │
              ▼
          Build .NET
              │
              ▼
          Run xUnit
              │
              ▼
      Run integration tests
              │
              ▼
       Build Docker image
              │
              ▼
           Deploy
```

Now your project starts looking like something you'd encounter in a professional development environment.

---

# 14. Eventually deploy it

Potential architecture:

```text
                    Azure
                      │
              ┌───────┴────────┐
              │                │
        App Service         Azure SQL
              │
        ASP.NET Core
              │
        Application Insights
```

You don't need to make the frontend complicated.

The point is to be able to tell an interviewer:

> "I developed the API in ASP.NET Core, persisted data through EF Core into SQL Server, containerised the application, created automated tests and deployed it to Azure."

That's an **excellent junior .NET developer story**.

---

# 15. Add a WCA-standard scramble generator — final milestone

Keep the existing basic 3×3 scramble generator while building the rest of the platform.

After the other milestones are complete, replace it with a scramble generator suitable for WCA-style practice. Start with 3×3 and use a proven implementation of the relevant random-state scrambling method.

Include:

* integrate the generator with the timer
* verify the generated scrambles and event-specific behaviour
* add automated tests
* consider additional events later

Avoid treating the current random-move generator as WCA-standard.

---

# The development roadmap I'd actually follow

Don't attempt all of this at once.

## Progress tracker

Use this section as the main project checklist. Mark items with `[x]` as you finish them, and leave `[ ]` for work that is still planned.

### ✅ Phase 1 — .NET fundamentals

**Goal:** get a basic API running.

Learn:

* [x] C#
* [x] ASP.NET Core
* [x] controllers
* [x] routing
* [x] dependency injection
* [x] DTO/request models

Build:

* [x] `GET /api/solves`
* [x] `GET /api/solves/{id}`
* [x] `POST /api/solves`
* [x] `PUT /api/solves/{id}`
* [x] `DELETE /api/solves/{id}`
* [x] `DELETE /api/solves`
* [x] `GET /api/solves/statistics`

---

### 🟡 Phase 2 — Database

Learn:

* [x] EF Core
* [ ] SQL Server
* [x] migrations
* [ ] relationships
* [x] LINQ

Build:

* [ ] User
* [ ] Session
* [x] Solve
* [x] Solve persistence through EF Core
* [x] Non-negative solve time database constraint

Note: the current implementation uses PostgreSQL via `UseNpgsql`, even though the original plan recommended SQL Server for stronger Microsoft/.NET portfolio alignment.

---

### 🟡 Phase 3 — Frontend

React + TypeScript.

Build:

* [x] solve entry
* [x] timer flow
* [x] solve history
* [x] statistics display
* [x] delete solve from UI
* [ ] charts

---

### 🟡 Phase 4 — Cube functionality

Add:

* [x] basic 3×3 scramble generation (WCA-standard generation is deferred to Phase 8)
* [x] Ao5
* [x] Ao12
* [x] Ao50
* [x] Ao100
* [x] PBs
* [x] total average
* [ ] Ao5 history
* [ ] Ao12 history
* [ ] rolling averages
* [ ] best/worst sessions
* [ ] improvement over time
* [ ] sessions
* [ ] algorithm database

---

### 🟡 Phase 5 — Testing

Add:

* [x] xUnit
* [x] unit tests for `Solve.FinalTime()`
* [x] unit tests for statistics calculations
* [x] controller tests with EF Core in-memory database
* [ ] full API integration tests with `WebApplicationFactory`
* [ ] frontend tests
* [ ] Playwright

---

### ⚪ Phase 6 — Security

Add:

* [ ] authentication
* [ ] JWT
* [ ] authorisation
* [x] basic input validation for creating solves
* [ ] validation for updating solves
* [ ] consistent validation error responses

---

### ⚪ Phase 7 — Production engineering

Add:

* [ ] structured logging
* [ ] global error handling
* [ ] health checks
* [ ] Docker
* [ ] Docker Compose
* [ ] CI/CD
* [x] OpenAPI endpoint in development
* [ ] Swagger UI / API documentation polish
* [ ] Azure

---

### ⚪ Phase 8 — WCA-standard scramble generation (final milestone)

Complete this after the other project phases.

* [ ] choose a proven WCA-style 3×3 random-state scramble implementation
* [ ] replace the basic scramble generator and integrate it with the timer
* [ ] verify scramble correctness and event-specific behaviour
* [ ] add automated tests for the generator and integration
* [ ] consider additional WCA events later

---
