namespace QuestionService.Services;

public interface ITagService
{
    Task<bool> AreTagValidAsync(List<string> slugs);
}