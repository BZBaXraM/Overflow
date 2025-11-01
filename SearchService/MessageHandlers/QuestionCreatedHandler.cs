using Contracts;

namespace SearchService.MessageHandlers;

public class QuestionCreatedHandler
{
    private readonly ITypesenseClient _client;

    public QuestionCreatedHandler(ITypesenseClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(QuestionCreated questionCreated)
    {
        var date = new DateTimeOffset(questionCreated.Created).ToUnixTimeMilliseconds();

        SearchQuestion question = new()
        {
            Id = questionCreated.QuestionId,
            Title = questionCreated.Title,
            Content = StripHtml(questionCreated.Content),
            CreatedAt = date,
            Tags = questionCreated.Tags.ToArray()
        };

        await _client.CreateDocument("questions", question);

        Console.WriteLine($"Created questin with id {questionCreated.QuestionId}");
    }

    private static string StripHtml(string content)
    {
        return Regex.Replace(content, "<.*?>", string.Empty);
    }
}