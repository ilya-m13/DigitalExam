using System.ComponentModel.DataAnnotations;

namespace DigitalExam.Models;

public class User
{
    [Key] public int Id { get; set; }

    [Required] public string Username { get; set; } = null!;
    [Required] public string Password { get; set; } = null!;

    public virtual ICollection<Exam> CreateadExams { get; set; } = new List<Exam>();
    public virtual ICollection<UserExam> PassedExams { get; set; } = new List<UserExam>();
}
