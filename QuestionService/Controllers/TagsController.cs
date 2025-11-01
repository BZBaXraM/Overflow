namespace QuestionService.Controllers;

[Authorize]
[Route("api/[controller]")]
[ApiController]
public class TagsController : ControllerBase
{
    private readonly QuestionContext _context;

    public TagsController(QuestionContext context)
    {
        _context = context;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Tag>>> GetTags()
    {
        return Ok(await _context.Tags.OrderBy(x => x.Name).ToListAsync());
    }
}