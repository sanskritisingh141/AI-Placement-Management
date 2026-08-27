namespace AIPlacement.Application.Students.DTOs;

public class StudentDto
{
    public int StudentId { get; set; }

    public int UserId { get; set; }

    public string? RollNo { get; set; }

    public string? Branch { get; set; }

    public decimal? CGPA { get; set; }

    public int? GraduationYear { get; set; }

    public int? CurrentBacklogs { get; set; }

    public string? Phone { get; set; }

    public DateTime? DateOfBirth { get; set; }
}
