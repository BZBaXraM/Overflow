using Contracts;

namespace SearchService.MessageHandlers;

public class AnswerCountUpdatedHandler
{
    private readonly ITypesenseClient _client;

    public AnswerCountUpdatedHandler(ITypesenseClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(AnswerCountUpdated message)
    {
        await _client.UpdateDocument("questions", message.QuestionId,
            new { message.AnswerCount }
        );
    }
}