# Math Test (Blazor Server)

Blazor Server app for importing math exams from XML, evaluating expressions, persisting graded results, and letting **teachers** and **students** review them. The solution is layered: **MathTest.Web** (UI + HTTP APIs), **Application** (MediatR commands/queries), **Infrastructure** (EF Core, SQL Server, XML parser), **Domain**, **MathEngine**, **SharedKernel**.

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [SQL Server LocalDB](https://learn.microsoft.com/sql/database-engine/configure-windows/sql-server-express-localdb) (default connection string targets `(localdb)\mssqllocaldb`)
- A database named **`MathTestDb`** (or adjust the connection string) with schema matching the SQL project **`services/Database/MathTest.Database`** — deploy the **MathTest.Database** dacpac or run the provided table scripts so `Users`, `Roles`, `Exams`, `ExamTasks`, etc. exist before first run.

---

## Configuration

| Setting | Purpose |
|--------|---------|
| `ConnectionStrings:DefaultConnection` | SQL Server connection (`appsettings.json`). |
| `Integration:ApiKey` | Shared secret for the **integration** upload API. If empty, integration routes return **503**. When set, must be **≥ 8 characters** (validated at startup). Development sample: `appsettings.Development.json`. |

Use [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables in production; do not commit real API keys.

---

## How to run

From the repository root (or any directory), run the web project:

```bash
dotnet run --project services/MathTest/MathTest.Web/MathTest.Web.csproj --launch-profile http
```

Or open `MathTest.Web` in Visual Studio / Rider and start the **http** profile.

### URLs (default profile)

| URL | Notes |
|-----|--------|
| `http://localhost:5069` | HTTP |
| `https://localhost:7264` | HTTPS (dev certificate; browser or `curl -k` may be needed) |

---

## What the app does

### Authentication & roles

- **Register** / **Sign in** with cookie authentication.
- Demo **external IDs** are validated on registration (see `Application/ExamImport/PredefinedExternalIds.cs`): teachers `T-10001`…`T-10003`, students `S-20001`…`S-20005`.
- **Teacher** and **Student** roles drive the nav and APIs.

### Teachers

- **`/teacher`** — Short hub with links to class exams and upload.
- **`/teacher/upload-exam-xml`** — Upload a `.xml` batch (multipart field **`file`**). Same handler as the integration route below.
- **`/teacher/exams`** — **Class exams** UI: lists exams linked to the logged-in teacher (`TeacherUserId`), shows **student external id**, score, and per-task correctness.
- **`GET /api/teacher/exams`** — JSON for the same data (used by the teacher analytics page).

### Students

- **`/student/results`** — **My exams** UI: exams linked to the logged-in student, with per-task correctness.
- **`GET /api/student/exams`** — JSON for student exam results (MediatR: `GetStudentExamsQuery`).

### Exam XML processing

- XML is parsed into a batch (teacher id, per-student exams and tasks).
- The **math engine** evaluates each expression and compares to the student’s numeric answer.
- Results are stored as **`Exam`** + **`ExamTask`** rows (scores, `IsCorrect`, etc.).
- **Duplicate imports** are skipped when the same **trimmed** triple already exists: **`ExternalTeacherId` + `ExternalId` (exam) + `ExternalStudentId`**, or the same triple appears twice in one file.

### Integration API (third-party / automation)

- **`POST /integration/v1/teacher/exams/batch-xml`**
- Header: **`X-Api-Key`** set to the same value as **`Integration:ApiKey`** in configuration
- Body: `multipart/form-data` with field **`file`** (XML).
- Same processing as the teacher UI upload; no interactive teacher cookie required.

Example (HTTPS, ignore dev cert):

```bash
curl -k -X POST "https://localhost:7264/integration/v1/teacher/exams/batch-xml" ^
  -H "X-Api-Key: change-me-local-integration-key" ^
  -F "file=@C:\path\to\batch-exam-sample.xml"
```

(Replace the key with your configured value; under Development the sample key may match `appsettings.Development.json`.)

### Other HTTP APIs

- **`/api/auth/register`**, **`/api/auth/login`**, **`/api/auth/logout`** — Registration and cookie sign-in/out.
- **`POST /api/teacher/exams/batch-xml`** — Teacher-only cookie session; same XML upload behavior as integration.

---

## Project layout (short)

| Project | Role |
|---------|------|
| **MathTest.Web** | Blazor UI, minimal APIs, auth wiring |
| **Application** | Commands (e.g. `ProcessExamXmlCommand`), queries (`GetStudentExamsQuery`, `GetTeacherExamsQuery`), ports (`IExamRepository`, `IStudentExamResultsQuery`, …) |
| **Infrastructure** | EF Core `AppDbContext`, repositories, XML parser, DI |
| **Domain** | Entities (`Exam`, `ExamTask`, `User`, …) |
| **MathEngine** | Expression evaluation used at import time |

---

## Troubleshooting

- **Port already in use** — Another instance is bound to `5069` / `7264`; stop it or change URLs via `dotnet run --urls "http://localhost:5071"`.
- **Database errors on startup** — Ensure SQL Server is running, the database exists, and the connection string is correct.
- **Integration returns 503** — Set `Integration:ApiKey` (non-empty, ≥ 8 characters).
- **401 on integration** — Missing or wrong `X-Api-Key` header.
