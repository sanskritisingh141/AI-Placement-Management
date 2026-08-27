param(
    [string]$ApiBaseUrl = "http://localhost:5158",
    [Parameter(Mandatory = $true)][string]$AdminEmail,
    [Parameter(Mandatory = $true)][string]$AdminPassword,
    [string]$ResumePdfPath
)

$ErrorActionPreference = "Stop"

function Invoke-JsonApi {
    param([string]$Method, [string]$Path, [object]$Body, [string]$Token)
    $parameters = @{ Method = $Method; Uri = "$($ApiBaseUrl.TrimEnd('/'))$Path" }
    if ($Token) { $parameters.Headers = @{ Authorization = "Bearer $Token" } }
    if ($null -ne $Body) {
        $parameters.ContentType = "application/json"
        $parameters.Body = $Body | ConvertTo-Json -Depth 10
    }
    try {
        Invoke-RestMethod @parameters
    }
    catch {
        $statusCode = $_.Exception.Response.StatusCode.value__
        $details = $_.ErrorDetails.Message

        if ([string]::IsNullOrWhiteSpace($details) -and $_.Exception.Response) {
            try {
                $reader = [System.IO.StreamReader]::new(
                    $_.Exception.Response.GetResponseStream())
                $details = $reader.ReadToEnd()
                $reader.Dispose()
            }
            catch {
                $details = $_.Exception.Message
            }
        }

        throw "$Method $Path failed with HTTP $statusCode. $details"
    }
}

function Assert-True([bool]$Condition, [string]$Message) {
    if (-not $Condition) { throw "E2E assertion failed: $Message" }
}

$suffix = [DateTimeOffset]::UtcNow.ToUnixTimeMilliseconds()
$password = "E2e-Strong-$suffix!"

Write-Host "1/10 Logging in as the seeded administrator..."
$admin = Invoke-JsonApi POST "/api/auth/login" @{ email = $AdminEmail; password = $AdminPassword }

Write-Host "2/10 Registering isolated Student and Company accounts..."
$student = Invoke-JsonApi POST "/api/auth/register" @{
    name = "E2E Student"; email = "e2e.student.$suffix@example.test"; password = $password
    role = "Student"; rollNo = "E2E$suffix"; branch = "Computer Science"
    cgpa = 9.1; graduationYear = 2027; currentBacklogs = 0
}
$company = Invoke-JsonApi POST "/api/auth/register" @{
    name = "E2E Recruiter"; email = "e2e.company.$suffix@example.test"; password = $password
    role = "Company"; companyName = "E2E Company $suffix"
}
$studentId = [int]$student.user.profileId
$companyId = [int]$company.user.profileId
Assert-True ($studentId -gt 0) "Student registration did not create a profile."
Assert-True ($companyId -gt 0) "Company registration did not create a profile."

Write-Host "3/10 Creating/reusing the required skill..."
$studentSkill = Invoke-JsonApi POST "/api/Skills" @{
    skillName = "CSharp-E2E-$suffix"; proficiencyLevel = "Advanced"
} $student.token
$skillId = [int]$studentSkill.skillId
Assert-True ($skillId -gt 0) "Skill creation did not return an ID."

Write-Host "4/10 Creating, approving, and publishing a Job Drive..."
$job = Invoke-JsonApi POST "/api/job-drives" @{
    companyId = $companyId; jobTitle = "E2E Software Engineer"
    jobDescription = "Automated full lifecycle verification role."; location = "Remote"
    minCGPA = 7.0; maxBacklogs = 0; graduationYear = 2027; salaryPackage = 12.5
    applicationDeadline = [DateTime]::UtcNow.AddDays(14).ToString("o")
    requiredSkillIds = @($skillId); eligibleBranches = @("Computer Science")
} $company.token
$jobDriveId = [int]$job.jobDriveId
$approved = Invoke-JsonApi PATCH "/api/admin/job-drives/$jobDriveId/approve" $null $admin.token
Assert-True ($approved.approvalStatus -eq "Approved") "Admin approval was not persisted."
Invoke-JsonApi PATCH "/api/job-drives/$jobDriveId/publish" $null $company.token | Out-Null

Write-Host "5/10 Verifying availability and eligibility..."
$available = @(Invoke-JsonApi GET "/api/job-drives" $null $null)
Assert-True ($available.jobDriveId -contains $jobDriveId) "Approved Job Drive is not publicly available."
$eligibility = Invoke-JsonApi GET "/api/job-drives/$jobDriveId/check-eligibility/$studentId" $null $student.token
Assert-True ([bool]$eligibility.isEligible) "The prepared Student should be eligible."

if ($ResumePdfPath) {
    Write-Host "6/10 Uploading and analyzing the PDF resume..."
    if (-not (Test-Path -LiteralPath $ResumePdfPath -PathType Leaf)) { throw "Resume PDF not found: $ResumePdfPath" }
    Add-Type -AssemblyName System.Net.Http
    $client = New-Object System.Net.Http.HttpClient
    $client.DefaultRequestHeaders.Authorization = New-Object `
        System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", $student.token)
    $multipart = New-Object System.Net.Http.MultipartFormDataContent
    $fileStream = [System.IO.File]::OpenRead((Resolve-Path -LiteralPath $ResumePdfPath))
    $fileContent = New-Object System.Net.Http.StreamContent($fileStream)
    $fileContent.Headers.ContentType = New-Object `
        System.Net.Http.Headers.MediaTypeHeaderValue("application/pdf")
    $multipart.Add($fileContent, "file", [System.IO.Path]::GetFileName($ResumePdfPath))

    try {
        $uploadResponse = $client.PostAsync(
            "$($ApiBaseUrl.TrimEnd('/'))/api/Resumes/upload",
            $multipart).Result
        $uploadBody = $uploadResponse.Content.ReadAsStringAsync().Result
        if (-not $uploadResponse.IsSuccessStatusCode) {
            throw "POST /api/Resumes/upload failed with HTTP $([int]$uploadResponse.StatusCode). $uploadBody"
        }
        $resume = $uploadBody | ConvertFrom-Json
    }
    finally {
        $multipart.Dispose()
        $client.Dispose()
    }
    Invoke-JsonApi POST "/api/ai/resumes/$($resume.resumeId)/analyze" $null $student.token | Out-Null
    $match = Invoke-JsonApi POST "/api/ai/job-drives/$jobDriveId/match" $null $student.token
    Assert-True ([int]$match.matchId -gt 0) "AI matching was not persisted."
    Assert-True ([int]$match.studentId -eq $studentId) "AI match belongs to the wrong Student."
    Assert-True ([int]$match.jobDriveId -eq $jobDriveId) "AI match belongs to the wrong Job Drive."
    Assert-True ([int]$match.resumeId -eq [int]$resume.resumeId) "AI match belongs to the wrong resume."
    Assert-True ([decimal]$match.match_score -ge 0 -and [decimal]$match.match_score -le 100) `
        "AI matching did not return a valid score."
} else { Write-Host "6/10 Resume/AI check skipped; pass -ResumePdfPath to include it." }

Write-Host "7/10 Applying and verifying Company applicant access..."
$application = Invoke-JsonApi POST "/api/recruitment/applications" @{ jobDriveId = $jobDriveId } $student.token
$applicationId = [int]$application.applicationId
$applicants = @(Invoke-JsonApi GET "/api/recruitment/job-drives/$jobDriveId/applicants" $null $company.token)
Assert-True ($applicants.applicationId -contains $applicationId) "Company cannot retrieve its applicant."

Write-Host "8/10 Running shortlist and interview workflow..."
Invoke-JsonApi PATCH "/api/recruitment/applications/$applicationId/status" @{ status = "Shortlisted"; remarks = "E2E shortlist" } $company.token | Out-Null
$round = Invoke-JsonApi POST "/api/recruitment/interview-rounds" @{
    jobDriveId = $jobDriveId; roundName = "Technical Round"; roundType = "Technical"; sequenceNo = 1
} $company.token
$interview = Invoke-JsonApi POST "/api/recruitment/interviews" @{
    applicationId = $applicationId; roundId = [int]$round.roundId
    scheduledAt = [DateTime]::UtcNow.AddDays(2).ToString("o"); location = "Online"
    meetingLink = "https://example.test/interview/$suffix"
} $company.token
Invoke-JsonApi POST "/api/recruitment/interviews/$($interview.interviewId)/result" @{
    result = "Passed"; score = 88; remarks = "E2E interview passed"
} $company.token | Out-Null

Write-Host "9/10 Selecting the Student and verifying placement creation..."
Invoke-JsonApi PATCH "/api/recruitment/applications/$applicationId/status" @{
    status = "Selected"; remarks = "E2E offer issued"
} $company.token | Out-Null
$placements = @(Invoke-JsonApi GET "/api/admin/placements" $null $admin.token)
Assert-True ($placements.applicationId -contains $applicationId) "Selection did not create a Placement record."

Write-Host "10/10 Verifying Admin analytics..."
$summary = Invoke-JsonApi GET "/api/admin/analytics/summary" $null $admin.token
Assert-True ([int]$summary.totalStudents -ge 1) "Admin analytics did not count Students."
Assert-True ([int]$summary.totalCompanies -ge 1) "Admin analytics did not count Companies."
Assert-True ([int]$summary.totalJobDrives -ge 1) "Admin analytics did not count Job Drives."
Assert-True ([int]$summary.totalApplications -ge 1) "Admin analytics did not count Applications."
Assert-True ([int]$summary.selectedStudents -ge 1) "Admin analytics did not count selected Students."

Write-Host "SUCCESS: database-backed Student -> Company -> Admin lifecycle passed." -ForegroundColor Green
Write-Host "JobDriveId=$jobDriveId, ApplicationId=$applicationId, StudentId=$studentId, CompanyId=$companyId"
