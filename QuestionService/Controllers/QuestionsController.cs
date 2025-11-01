namespace QuestionService.Controllers;

// [Authorize]
[Route("api/[controller]")]
[ApiController]
public class QuestionsController : ControllerBase
{
    private readonly QuestionContext _context;
    private readonly IMessageBus _bus;
    private readonly ITagService _tagService;

    public QuestionsController(QuestionContext context, IMessageBus bus, ITagService tagService)
    {
        _context = context;
        _bus = bus;
        _tagService = tagService;
    }
    
    [Authorize]
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
    
    [Authorize]
    [HttpPost]
    public async Task<ActionResult<Question>> CreateQuestion(CreateQuestionRequest request)
    {
        if (!await _tagService.AreTagValidAsync(request.Tags))
        {
            return BadRequest("Invalid tags");
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "test-user-id";
        var name = User.FindFirstValue("name") ?? "Test User";

        // if (userId is null || name is null) return BadRequest("Cannot get user details");

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

        await _bus.PublishAsync(new QuestionCreated(question.Id, question.Title, question.Content, question.CreatedAt,
            question.TagSlugs));

        return CreatedAtAction(nameof(GetById), new { id = question.Id }, question);
    }

    [Authorize]
    [HttpGet("{id}")]
    public async Task<ActionResult<Question>> GetById([FromRoute] string id)
    {
        var question = await _context.Questions
            .Include(x => x.Answers.OrderByDescending(a => a.Accepted).ThenByDescending(a => a.CreatedAt))
            .FirstOrDefaultAsync(x => x.Id == id);

        if (question is null) return NotFound();

        await _context.Questions.Where(x => x.Id == id)
            .ExecuteUpdateAsync(setters => setters.SetProperty(x => x.ViewCount,
                x => x.ViewCount + 1));

        return Ok(question);
    }

    [Authorize]
    [HttpGet("{questionId}/answers/{answerId}")]
    public async Task<ActionResult<Answer>> GetAnswerById(string questionId, string answerId)
    {
        var answer = await _context.Answers
            .FirstOrDefaultAsync(a => a.Id == answerId && a.QuestionId == questionId);

        if (answer is null) return NotFound();

        return Ok(answer);
    }

    [Authorize]
    [HttpPut("{id}")]
    public async Task<ActionResult<Question>> UpdateQuestion([FromRoute] string id,
        [FromBody] UpdateQuestionRequest request)
    {
        if (!await _tagService.AreTagValidAsync(request.Tags))
        {
            return BadRequest("Invalid tags");
        }

        var question = await _context.Questions.FirstOrDefaultAsync(x => x.Id == id);

        if (question is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "test-user-id";

        if (userId != question.AskerId) return Forbid();

        question.Title = request.Title;
        question.Content = request.Content;
        question.TagSlugs = request.Tags;
        question.UpdateAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();


        await _bus.PublishAsync(new QuestionCreated(question.Id, question.Title, question.Content, question.CreatedAt,
            question.TagSlugs));

        return NoContent();
    }

    [Authorize]
    [HttpDelete("{id}")]
    public async Task<ActionResult<Question>> DeleteQuestion([FromRoute] string id)
    {
        var question = await _context.Questions.FindAsync(id);

        if (question is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "test-user-id";

        if (userId != question.AskerId) return Forbid(); // Отключено для тестирования

        _context.Questions.Remove(question);
        await _context.SaveChangesAsync();

        await _bus.PublishAsync(new QuestionDeleted(question.Id));

        return NoContent();
    }

    [Authorize]
    [HttpPost("{questionId}/answers")]
    public async Task<ActionResult> PostAnswer(string questionId, CreateAnswerRequest request)
    {
        var question = await _context.Questions
            .Include(q => q.Answers)
            .FirstOrDefaultAsync(q => q.Id == questionId);

        if (question is null) return NotFound();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "test-user-id";
        var name = User.FindFirstValue("name") ?? "Test User";


        Answer answer = new()
        {
            Content = request.Content,
            UserId = userId,
            UserDisplayName = name,
            QuestionId = questionId
        };

        _context.Answers.Add(answer);
        question.AnswerCount++;

        await _context.SaveChangesAsync();

        await _bus.PublishAsync(new AnswerCountUpdated(questionId, question.AnswerCount));

        return CreatedAtAction(nameof(GetAnswerById), new { questionId, answerId = answer.Id }, answer);
    }

    [Authorize]
    [HttpPut("{questionId}/answers/{answerId}")]
    public async Task<ActionResult> UpdateAnswer(string questionId, string answerId,
        CreateAnswerRequest request)
    {
        var answer = await _context.Answers.FindAsync(answerId);
        if (answer is null) return NotFound();
        if (answer.QuestionId != questionId) return BadRequest("Answer does not belong to this question");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "test-user-id";
        if (userId != answer.UserId) return Forbid(); // Отключено для тестирования

        answer.Content = request.Content;
        answer.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();
        return NoContent();
    }

    [Authorize]
    [HttpDelete("{questionId}/answers/{answerId}")]
    public async Task<ActionResult> DeleteAnswer(string questionId, string answerId)
    {
        var answer = await _context.Answers.FindAsync(answerId);
        var question = await _context.Questions.FindAsync(questionId);

        if (answer is null || question is null) return NotFound();
        if (answer.QuestionId != questionId) return BadRequest("Answer does not belong to this question");
        if (answer.Accepted) return BadRequest("Cannot delete accepted answer");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "test-user-id";
        if (userId != answer.UserId && userId != question.AskerId) return Forbid(); // Отключено для тестирования

        _context.Answers.Remove(answer);
        question.AnswerCount--;

        await _context.SaveChangesAsync();
        await _bus.PublishAsync(new AnswerCountUpdated(questionId, question.AnswerCount));

        return NoContent();
    }

    [Authorize]
    [HttpPost("{questionId}/answers/{answerId}/accept")]
    public async Task<ActionResult> AcceptAnswer(string questionId, string answerId)
    {
        var answer = await _context.Answers.FindAsync(answerId);
        var question = await _context.Questions.FindAsync(questionId);

        if (answer is null || question is null) return NotFound();
        if (answer.QuestionId != questionId) return BadRequest("Answer does not belong to this question");
        if (question.HasAcceptedAnswer) return BadRequest("Question already has an accepted answer");

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "test-user-id";
        if (userId != question.AskerId) return Forbid(); // Отключено для тестирования

        var otherAcceptedAnswers = await _context.Answers
            .Where(a => a.QuestionId == questionId && a.Accepted)
            .ToListAsync();

        foreach (var otherAnswer in otherAcceptedAnswers)
        {
            otherAnswer.Accepted = false;
        }

        answer.Accepted = true;
        question.HasAcceptedAnswer = true;

        await _context.SaveChangesAsync();

        await _bus.PublishAsync(new AnswerAccepted(questionId));
        return NoContent();
    }

    [HttpGet("errors")]
    public ActionResult GetErrorResponses(int code)
    {
        ModelState.AddModelError("Problem one", "Validation problem one");
        ModelState.AddModelError("Problem two", "Validation problem two");

        return code switch
        {
            400 => BadRequest("Opposite of good request"),
            401 => Unauthorized(),
            403 => Forbid(),
            404 => NotFound(),
            500 => throw new Exception("This is a server error"),
            _ => ValidationProblem(ModelState)
        };
    }
}