using Contracts;

namespace SearchService.MessageHandlers;

public class AcceptAnswerHandler
{
    private readonly ITypesenseClient _client;

    public AcceptAnswerHandler(ITypesenseClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(AnswerAccepted message)
    {
        await _client.UpdateDocument("questions", message.QuestionId,
            new { HasAcceptedAnswer = true });
    }
}