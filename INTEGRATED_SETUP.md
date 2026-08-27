# Integrated setup and verification

## Prerequisites

- .NET 8 SDK
- SQL Server Express at `localhost\SQLEXPRESS`
- Python 3.11+

## Database

From the repository root:

```powershell
dotnet ef database update --project src/AIPlacement.Infrastructure --startup-project src/AIPlacement.API
```

The migration adds `CurrentBacklogs` and all existing migrations create the shared `AIPlacementDb`. Do not create a second database.

## Admin account

Set seed credentials in the shell before starting the API or MVC application. They are only used when the email does not already exist:

```powershell
$env:SeedAdmin__Email = "admin@placement.local"
$env:SeedAdmin__Password = "Choose-A-Strong-Password"
```

For production, also set `Jwt__Key` to a secret random value of at least 32 bytes. Never commit it.

## AI service

```powershell
py -m venv .venv
.\.venv\Scripts\Activate.ps1
pip install -r src/AIPlacement.AI/requirements.txt
uvicorn app.main:app --app-dir src/AIPlacement.AI --port 8000
```

Verify `http://localhost:8000/health` returns `{"status":"ok"}`.

## API and MVC

Run these in separate terminals:

```powershell
dotnet run --project src/AIPlacement.API --launch-profile http
dotnet run --project src/AIPlacement.MVC --launch-profile http
```

## End-to-end scenario

1. Register one Company and one Student through MVC.
2. Sign in as Company, complete the company profile, create a job with existing Skill IDs and eligibility criteria, then publish it.
3. Sign in as Admin and approve the pending job drive.
4. Sign in as Student, complete profile/backlogs, add required skills, and upload a valid PDF resume.
5. Analyze the resume, open the approved job, calculate its match score, confirm eligibility, and apply.
6. Sign in as Company, review the applicant, create an interview round, schedule the interview, record its result, and set the application to Selected.
7. Sign in as Student and confirm the application status. Sign in as Admin and confirm the placement and analytics records.

Run `dotnet build --no-restore` and `dotnet test --no-restore` before committing.

The same scenario can be run automatically from a separate PowerShell 7 window
while the API and FastAPI service are running:

```powershell
.\scripts\verify-e2e.ps1 `
  -AdminEmail $env:SeedAdmin__Email `
  -AdminPassword $env:SeedAdmin__Password `
  -ResumePdfPath "C:\path\to\resume.pdf"
```

The runner creates unique test users and data, so it is safe to run repeatedly.
Omit `ResumePdfPath` only when intentionally skipping the PDF/AI stage.
