using DigitalExam.Data;
using DigitalExam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Principal;

namespace DigitalExam.Controllers;

[Authorize]
public class UserController : Controller
{
    private readonly DigitalExamContext _context;

    public UserController(DigitalExamContext context) => _context = context;

    public async Task<IActionResult> Index()
    {
        return View(await _context.Users
            .Include(e => e.CreateadExams)
            .Include(e => e.PassedExams)
                .ThenInclude(e => e.Exam)
            .FirstAsync(e => e.Username == UserName()));
    }

    [HttpGet]
    public IActionResult CreateExam()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> CreateExam(string name)
    {
        var exam = new Exam()
        {
            Name = name,
            Author = await _context.Users.FirstAsync(e => e.Username == UserName())
        };

        await _context.Exams.AddAsync(exam);
        await _context.SaveChangesAsync();

        ViewBag.ExamId = exam.Id;

        return View(nameof(CreateQuestions));
    }

    [HttpPost]
    public async Task<IActionResult> CreateQuestions(int exam_id, Question qst)
    {
        var exam = await _context.Exams.Include(e => e.Questions).FirstAsync(e => e.Id == exam_id);
        if (exam == null)
        {
            return NotFound();
        }

        exam.Questions.Add(qst);
        await _context.SaveChangesAsync();

        ViewBag.ExamId = exam_id;
        ViewBag.AnsNum = 0;
        ViewBag.QstCnt = exam.Questions.Count;

        return View();
    }

    [HttpPost]
    public IActionResult EditAnsNum(int id, int ans_num)
    {
        ViewBag.ExamId = id;
        ViewBag.AnsNum = ans_num;

        return View(nameof(CreateQuestions));
    }

    [HttpGet]
    public IActionResult ConfirmedCreateExam(int id)
    {
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> EditExam(int id)
    {
        return View(await _context.Exams
            .Include(e => e.Questions)
            .FirstOrDefaultAsync(e => e.Id == id));
    }

    [HttpPost]
    public async Task<IActionResult> EditExam(int exam_id, Exam exam)
    {
        var edited_exam = await _context.Exams.Include(e => e.Questions).FirstOrDefaultAsync(e => e.Id == exam_id);
        if (edited_exam == null)
        {
            return NotFound();
        }

        edited_exam.Name = exam.Name;

        await _context.SaveChangesAsync();

        return View(await _context.Exams
            .Include(e => e.Questions)
            .FirstAsync(e => e.Id == exam_id));
    }

    [HttpGet]
    public async Task<IActionResult> EditQuestion(int qst_id)
    {
        return View(await _context.Questions.FindAsync(qst_id));
    }

    [HttpPost]
    public async Task<IActionResult> EditQuestion(int qst_id, Question question)
    {
        var edited_qst = await _context.Questions.FindAsync(qst_id);
        if (edited_qst == null)
        {
            return NotFound();
        }

        edited_qst.Title = question.Title;
        edited_qst.Answers = question.Answers;
        edited_qst.CorrectAnswer = question.CorrectAnswer;

        await _context.SaveChangesAsync();

        return View(nameof(EditExam),
            await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == edited_qst.ExamId));
    }

    [HttpGet]
    public async Task<IActionResult> DeleteQuestion(int exam_id, int qst_id)
    {
        var qst = await _context.Questions.FindAsync(qst_id);
        if (qst == null)
        {
            return NotFound();
        }

        _context.Questions.Remove(qst);
        await _context.SaveChangesAsync();

        return View(nameof(EditExam),
            await _context.Exams
                .Include(e => e.Questions)
                .FirstOrDefaultAsync(e => e.Id == exam_id));
    }

    [HttpGet]
    public async Task<IActionResult> DeleteExam(int id)
    {
        return View(await _context.Exams.FindAsync(id));
    }

    [HttpPost, ActionName("DeleteExam")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var exam = await _context.Exams.FindAsync(id);
        if (exam == null)
        {
            return NotFound();
        }

        _context.Exams.Remove(exam);
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> AddAnswer(int qst_id)
    {
        var qst = await _context.Questions.FindAsync(qst_id);
        if (qst == null)
        {
            return NotFound();
        }

        qst.Answers.Add("");

        await _context.SaveChangesAsync();

        return View(nameof(EditQuestion), qst);
    }

    public async Task<IActionResult> DeleteAnswer(int qst_id, int ans_id)
    {
        var qst = await _context.Questions.FindAsync(qst_id);
        if (qst == null)
        {
            return NotFound();
        }

        qst.Answers.RemoveAt(ans_id);

        await _context.SaveChangesAsync();

        return View(nameof(EditQuestion), await _context.Questions.FindAsync(qst_id));
    }

    private string UserName()
    {
        return HttpContext.User.Identity!.Name!;
    }
}
