using DigitalExam.Models;
using Microsoft.EntityFrameworkCore;
//using DigitalExam.Classes;

namespace DigitalExam.Data;

public class DigitalExamContext : DbContext
{
    public DigitalExamContext()
    {
        //Database.EnsureDeleted();
        Database.EnsureCreated();

        if (Users.Count() == 0)
        {
            {
                var user = new User()
                {
                    Username = "user1",
                    Password = "12345"
                };
                Users.Add(user);
                SaveChanges();
            }
            {
                var exam = new Exam()
                {
                    Name = "test1",
                    Author = Users.Find(1)!,
                    Questions = new List<Question>()
                    {
                        new () {Title = "title1", Answers = new List<string>() { "answer1", "cor" }, CorrectAnswer = "cor" },
                        new () {Title = "title2", Answers = new List<string>() { "cor", "answer2" }, CorrectAnswer = "cor" },
                    }
                };
                Exams.Add(exam);
                SaveChanges();
            }
            {
                var user = new User()
                {
                    Username = "user2",
                    Password = "12345"
                };
                Users.Add(user);
                SaveChanges();
            }
            {
                var exam = new Exam()
                {
                    Name = "test2",
                    Author = Users.Find(2)!,
                    Questions = new List<Question>()
                    {
                        new () {Title = "title1", Answers = new List<string>() { "cor", "answer2" }, CorrectAnswer = "cor" },
                        new () {Title = "title2", Answers = new List<string>() { "answer1", "cor" }, CorrectAnswer = "cor" },
                    }
                };
                Exams.Add(exam);
                SaveChanges();
            }
            {
                var userexam = new UserExam()
                {
                    User = Users.Find(1)!,
                    Exam = Exams.Find(2)!,
                    Grade = 0
                };
                UserExams.Add(userexam);
                SaveChanges();
            }
            {
                var userexam = new UserExam()
                {
                    User = Users.Find(2)!,
                    Exam = Exams.Find(1)!,
                    Grade = 0
                };
                UserExams.Add(userexam);
                SaveChanges();
            }
        }
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite("Data Source=.\\Data\\SQlLiteDatabase.db");
        base.OnConfiguring(optionsBuilder);
    }

    public virtual DbSet<User> Users { get; set; } = default!;
    public virtual DbSet<Exam> Exams { get; set; } = default!;
    public virtual DbSet<UserExam> UserExams { get; set; } = default!;
    public virtual DbSet<Question> Questions { get; set; } = default!;
}
