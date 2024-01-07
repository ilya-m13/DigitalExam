using System.ComponentModel.DataAnnotations;

namespace DigitalExam.Models;

public class Question
{
    [Key] public int Id { get; set; }

    public string Title { get; set; } = null!;
    public IList<string> Answers { get; set; } = new List<string>();
    public string CorrectAnswer { get; set; } = null!;

    [Required] public int ExamId { get; set; }
    public Exam Exam { get; set; } = null!;
}
