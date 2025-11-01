namespace QuestionService.Services;

public class TagService : ITagService
{
    private readonly IMemoryCache _cache;
    private readonly QuestionContext _context;
    private const string CacheKey = "tags";

    public TagService(IMemoryCache cache, QuestionContext context)
    {
        _cache = cache;
        _context = context;
    }

    private async Task<List<Tag>> GetTagsAsync()
    {
        return await _cache.GetOrCreateAsync(CacheKey, async entity =>
        {
            entity.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(2);

            var tags = await _context.Tags.AsNoTracking().ToListAsync();

            return tags;
        }) ?? [];
    }

    public async Task<bool> AreTagValidAsync(List<string> slugs)
    {
        var tags = await GetTagsAsync();
        
        var tagSet = tags.Select(x => x.Slug).ToHashSet(StringComparer.OrdinalIgnoreCase);
        
        return slugs.All(x => tagSet.Contains(x));
    }
}