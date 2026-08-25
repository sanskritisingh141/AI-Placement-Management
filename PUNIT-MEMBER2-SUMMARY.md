# Punit Kumar — Member 2 (Pair 2) — Functionality Summary

**Branch**: `punit-member2-jobs-recruitment`  
**Scope**: Jobs + Applications + Recruitment (Interviews)

---

## Jobs Module

### Job Drives (CRUD + Lifecycle)

| Endpoint | Method | Role | What it does |
|---|---|---|---|
| `GET /api/job-drives` | GET | Public | List all open, approved job drives |
| `GET /api/job-drives/{id}` | GET | Public | Get a single job drive with eligibility criteria, skills, branches |
| `GET /api/job-drives/company/{companyId}` | GET | Company/Admin | List all drives for a specific company |
| `POST /api/job-drives` | POST | Company | Create a new job drive (Draft + Pending approval) with eligibility criteria, required skills, eligible branches |
| `PUT /api/job-drives/{id}` | PUT | Company | Update a job drive (blocked if Closed) |
| `PATCH /api/job-drives/{id}/publish` | PATCH | Company | Move drive from Draft → Open (only if admin-approved) |
| `PATCH /api/job-drives/{id}/close` | PATCH | Company | Close a job drive |

**Supporting tables**: JobDrives, EligibilityCriteria, JobSkills, JobEligibleBranches

**Business rules enforced**:
- Drive starts as Draft with Pending approval
- Only admin-approved drives can be published
- Closed drives cannot be edited
- Validation on CGPA (0–10), graduation year, salary, deadline, etc.

---

## Applications Module

### Eligibility Check

| Endpoint | Method | Role | What it does |
|---|---|---|---|
| `GET /api/job-drives/{id}/check-eligibility/{studentId}` | GET | Student/Company/Admin | Check if a student meets the job drive's criteria |

**Checks performed**:
- CGPA >= minimum required
- Branch is in eligible branches list
- Graduation year matches
- Job drive is Open
- Application deadline has not passed
- Returns `IsEligible` boolean + list of `Reasons` if ineligible

### Application Submission

| Endpoint | Method | Role | What it does |
|---|---|---|---|
| `POST /api/recruitment/applications` | POST | Student | Apply to a job drive |

**Business rules enforced**:
- Eligibility is verified before accepting
- Duplicate applications are blocked
- Initial status = "Applied"
- Creates ApplicationStatusHistory record on submission

### Application Status Management

| Endpoint | Method | Role | What it does |
|---|---|---|---|
| `GET /api/recruitment/job-drives/{id}/applicants` | GET | Company/Admin | List all applicants for a drive (includes AI match scores) |
| `PATCH /api/recruitment/applications/{id}/status` | PATCH | Company/Admin | Update application status |

**Status pipeline**: Applied → Under Review → Shortlisted → Assessment → Technical Interview → HR Interview → Selected / Rejected

**Business rules enforced**:
- Selected/Rejected applications are locked (cannot change further)
- When status is set to "Selected", a PlacementResult record is automatically created with the job's salary package

---

## Recruitment / Interviews Module

| Endpoint | Method | Role | What it does |
|---|---|---|---|
| `POST /api/recruitment/interview-rounds` | POST | Company/Admin | Create an interview round for a job drive |
| `POST /api/recruitment/interviews` | POST | Company/Admin | Schedule an interview for an applicant |
| `POST /api/recruitment/interviews/{id}/result` | POST | Company/Admin | Record interview result (score 0–100) |

**Business rules enforced**:
- Interviews cannot be scheduled for Selected/Rejected applications
- Interview round must belong to the same job drive as the application
- Interview time must be in the future
- Only one result per interview (no duplicates)
- Recording a result marks the schedule as "Completed"

---

## Cross-cutting Concerns

- **Role-based authorization** (`[Authorize]`) on all endpoints — Student for applying, Company for mutations, Admin has access alongside Company. Public read endpoints are `[AllowAnonymous]`. Will activate once JWT auth middleware is configured by another pair.
- **AI Match Scores** from `JobMatchScores` table are included in the applicant list (batch-fetched, no N+1).
- **N+1 query optimization** — batch fetch for eligibility criteria, skills, and branches when listing job drives (4 queries total instead of 3N+1).
- **Transaction safety** — job drive creation (drive + criteria + skills + branches) wrapped in a DB transaction.
- **Status constants** — no magic strings; `JobDriveStatus`, `JobDriveApprovalStatus`, and `RecruitmentStatus` classes define all valid values.
- **PlacementResult auto-creation** — when a student is marked "Selected", a placement record is created automatically.

---

## Architecture (Clean Architecture)

```
API Layer (Controllers)
  └── Application Layer (Services + Interfaces + DTOs)
        └── Infrastructure Layer (Repositories + EF Core)
              └── Domain Layer (Entities)
```

### Files by layer

**API (Controllers)**:
- `JobDrivesController.cs` — 8 endpoints
- `RecruitmentController.cs` — 6 endpoints

**Application (Services / Interfaces / DTOs)**:
- `IJobDriveService.cs` / `JobDriveService.cs`
- `IRecruitmentService.cs` / `RecruitmentService.cs`
- `IJobDriveRepository.cs` / `IRecruitmentRepository.cs`
- DTOs: `JobDriveDto`, `CreateJobDriveDto`, `UpdateJobDriveDto`, `ApplicantDto`, `ApplyToJobDriveDto`, `EligibilityResultDto`, `UpdateApplicationStatusDto`, `InterviewRoundDto`, `InterviewScheduleDto`, `InterviewResultDto`, `CreateInterviewRoundDto`, `ScheduleInterviewDto`, `RecordInterviewResultDto`
- Constants: `JobDriveStatus`, `JobDriveApprovalStatus`, `RecruitmentStatus`

**Infrastructure (Repositories)**:
- `JobDriveRepository.cs`
- `RecruitmentRepository.cs`

**Domain (Entities — shared, not created by this branch)**:
- `JobDrive`, `EligibilityCriteria`, `JobSkill`, `JobEligibleBranch`
- `Application`, `ApplicationStatusHistory`
- `InterviewRound`, `InterviewSchedule`, `InterviewResult`
- `PlacementResult`, `JobMatchScore`, `StudentProfile`

---

## What's NOT on this branch (handled by other pairs)
- Authentication middleware / JWT setup
- Student profiles, resumes, skills CRUD
- Company profiles CRUD
- Admin approval workflow for job drives
- AI matching/scoring engine
- Analytics and placement reports
