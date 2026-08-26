namespace AIPlacement.Application.Recruitment;

public static class RecruitmentStatus
{
    public const string Applied = "Applied";
    public const string UnderReview = "Under Review";
    public const string Shortlisted = "Shortlisted";
    public const string Assessment = "Assessment";
    public const string TechnicalInterview = "Technical Interview";
    public const string HrInterview = "HR Interview";
    public const string Selected = "Selected";
    public const string Rejected = "Rejected";

    public static readonly IReadOnlySet<string> ValidStatuses =
        new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Applied,
            UnderReview,
            Shortlisted,
            Assessment,
            TechnicalInterview,
            HrInterview,
            Selected,
            Rejected
        };
}
