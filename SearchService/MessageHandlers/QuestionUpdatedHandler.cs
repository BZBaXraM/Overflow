using Contracts;

namespace SearchService.MessageHandlers;

public class QuestionUpdatedHandler
{
    private readonly ITypesenseClient _client;

    public QuestionUpdatedHandler(ITypesenseClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(QuestionUpdated questionUpdated)
    {
        SearchQuestion question = new()
        {
            Id = questionUpdated.QuestionId,
            Title = questionUpdated.Title,
            Content = StripHtml(questionUpdated.Content),
            Tags = questionUpdated.Tags.ToArray()
        };

        await _client.UpdateDocument("questions", question.Id, question);
    }

    private static string StripHtml(string content)
    {
        return Regex.Replace(content, "<.*?>", string.Empty);
    }
}