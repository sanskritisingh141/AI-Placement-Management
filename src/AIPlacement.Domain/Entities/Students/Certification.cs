namespace AIPlacement.Domain.Entities.Students
{
    public class Certification
    {
        public int CertificationId { get; set; }

        public int StudentId { get; set; }

        public string CertificateName { get; set; } = string.Empty;

        public string IssuingOrganization { get; set; } = string.Empty;

        public DateTime IssueDate { get; set; }

        public string? CredentialUrl { get; set; }
    }
}
