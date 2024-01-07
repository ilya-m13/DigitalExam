using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DigitalExam.Data;
using DigitalExam.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace DigitalExam.Controllers;

public class ExamsController : Controller
{
    private readonly DigitalExamContext _context;

    public ExamsController(DigitalExamContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<IActionResult> Index(string? sort)
    {
        var exams = await _context.Exams.Include(e => e.Author).ToListAsync();
        return sort switch
        {
            "Name" => View(exams.OrderBy(e => e.Name)),
            "Author" => View(exams.OrderBy(e => e.Author.Username)),
            _ => View(exams)
        };
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id)
    {
        return View(await _context.Exams
            .Include(e => e.PassedUsers)
                .ThenInclude(c => c.User)
            .Include(e => e.Author)
            .FirstOrDefaultAsync(e => e.Id == id));
    }

    [HttpGet]
    public async Task<IActionResult> RemoveExam(int id)
    {
        var exam = await _context.Exams.FindAsync(id);
        if (exam == null)
        {
            return NotFound();
        }
        _context.Exams.Remove(exam);
        await _context.SaveChangesAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Pass(int id)
    {
        return View(await _context.Exams
            .Include(e => e.Questions)
            .Include(e => e.Author)
            .FirstOrDefaultAsync(e => e.Id == id));
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> Passing(int exam_id, int question_idx)
    {
        var exam = await _context.Exams.Include(e => e.Questions).FirstOrDefaultAsync(e => e.Id == exam_id);
        if (exam == null)
        {
            return NotFound();
        }

        var question = exam.Questions.ToList()[question_idx];

        ViewData["QuestionIdx"] = question_idx;
        ViewData["crt_ans_cnt"] = 0;

        return View(exam);
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Passing(int exam_id, int question_idx, int answer_idx, int crt_ans_cnt)
    {
        var exam = await _context.Exams.Include(e => e.Questions).Include(e => e.PassedUsers).ThenInclude(e => e.User).FirstOrDefaultAsync(e => e.Id == exam_id);
        if (exam == null)
        {
            return NotFound();
        }

        var questions = exam.Questions.ToList();

        if (questions[question_idx].Answers[answer_idx] == questions[question_idx].CorrectAnswer)
        {
            crt_ans_cnt++;
        }

        ViewData["QuestionIdx"] = ++question_idx;
        ViewData["crt_ans_cnt"] = crt_ans_cnt;

        if (questions.Count == question_idx)
        {
            var details = exam.PassedUsers.FirstOrDefault(e => e.User.Username == HttpContext.User.Identity!.Name);
            if (details == null)
            {
                details = new UserExam()
                {
                    User = await _context.Users.FirstAsync(e => e.Username == HttpContext.User.Identity!.Name),
                    Exam = exam,
                    Grade = (double)crt_ans_cnt / questions.Count * 100.0
                };
                _context.UserExams.Add(details);
            } else
            {
                details.Grade = (double)crt_ans_cnt / questions.Count * 100.0;
            }

            await _context.SaveChangesAsync();

            ViewBag.Grade = details.Grade;

            return View("Reference");
        }

        return View(exam);
    }
}