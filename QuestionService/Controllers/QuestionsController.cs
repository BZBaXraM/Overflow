using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuestionService.Data;
using QuestionService.DTOs;
using QuestionService.Entities;

namespace QuestionService.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly QuestionContext _context;

    public QuestionsController(QuestionContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<List<Question>>> GetQuestions([FromQuery] string? tag)
    {
        var query = _context.Questions.AsQueryable();

        if (!string.IsNullOrEmpty(tag))
        {
            query = query.Where(x => x.TagSlugs.Contains(tag));
        }

        return await query.OrderByDescending(x => x.CreatedAt).ToListAsync();
    }

    [HttpPost]
    public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionRequest request)
    {
        var validTags = await _context.Tags
            .Where(x => request.Tags.Contains(x.Slug))
            .ToListAsync();

        var missing = request.Tags.Except(validTags.Select(x => x.Slug)
                .ToList())
            .ToList();

        if (missing.Count != 0)
            return BadRequest($"Invalid tags: {string.Join(", ", missing)}");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var name = User.FindFirstValue("name");

        if (userId is null || name is null) return BadRequest("Cannot get user details");

        Question question = new()
        {
            Title = request.Title,
            Content = request.Content,
            TagSlugs = request.Tags,
            AskerId = userId,
            AskerDisplayName = name
        };

        _context.Questions.Add(question);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetById), new { id = question.Id }, question);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetById([FromRoute] string id)
    {
        var item = await _context.Questions.FindAsync(id);

        if (item is null) return NotFound();

        await _context.Questions.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ViewCount,
                x => x.ViewCount + 1));

        return item;
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<Question>> UpdateQuestion([FromRoute] string id,
        [FromBody] UpdateQuestionRequest request)
    {
        var validTags = await _context.Tags
            .Where(x => request.Tags.Contains(x.Slug))
            .ToListAsync();

        var missing = request.Tags.Except(validTags.Select(x => x.Slug)
                .ToList())
            .ToList();

        if (missing.Count != 0)
            return BadRequest($"Invalid tags: {string.Join(", ", missing)}");

        var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);

        if (question is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != question.AskerId) return Forbid();

        question.Title = request.Title;
        question.Content = request.Content;
        question.TagSlugs = request.Tags;
        question.UpdateAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Question>> DeleteQuestion([FromRoute] string id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (userId != question.AskerId) return Forbid();

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}