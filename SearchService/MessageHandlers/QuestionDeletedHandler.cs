using Contracts;

namespace SearchService.MessageHandlers;

public class QuestionDeletedHandler
{
    private readonly ITypesenseClient _client;

    public QuestionDeletedHandler(ITypesenseClient client)
    {
        _client = client;
    }

    public async Task HandleAsync(QuestionDeleted questionDeleted)
    {
        await _client.DeleteDocument<SearchQuestion>("questions", questionDeleted.QuestionId);
    }
}