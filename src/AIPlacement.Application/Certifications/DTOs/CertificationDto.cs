namespace AIPlacement.Application.Certifications.DTOs;

public class CertificationDto
{
    public int CertificationId { get; set; }

    public int StudentId { get; set; }

    public string? CertificateName { get; set; }

    public string? IssuingOrganization { get; set; }

    public DateTime? IssueDate { get; set; }

    public string? CredentialUrl { get; set; }
}