namespace AIPlacement.Domain.Entities.Students
{
    public class StudentProfile
    {
        public int StudentId { get; set; }

        public int UserId { get; set; }

        public string RollNo { get; set; } = string.Empty;

        public string Branch { get; set; } = string.Empty;

        public decimal CGPA { get; set; }

        public int GraduationYear { get; set; }

        public string? Phone { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}