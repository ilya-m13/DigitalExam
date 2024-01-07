using System.ComponentModel.DataAnnotations;

namespace DigitalExam.Models;

public class UserExam
{
    [Key] public int Id { get; set; }

    [Required] public double Grade { get; set; }

    [Required] public int UserId { get; set; }
    public User User { get; set; } = null!;

    [Required] public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
}
