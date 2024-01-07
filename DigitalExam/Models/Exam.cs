using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
//using DigitalExam.Classes;

namespace DigitalExam.Models;

public class Exam
{
    [Key] public int Id { get; set; }

    [Required] public string Name { get; set; } = null!;

    public virtual ICollection<Question> Questions { get; set; } = new List<Question>();

    [Required] public int AuthorId { get; set; }
    public User Author { get; set; } = null!;

    public virtual ICollection<UserExam> PassedUsers { get; set; } = new List<UserExam>();
}
