namespace SearchService.Data;

public sealed class SearchInitializer
{
    public static async Task EnsureIndexExistsAsync(ITypesenseClient client)
    {
        const string schemaName = "questions";

        try
        {
            await client.RetrieveCollection(schemaName);
            Console.WriteLine($"Collection {schemaName} has been created already.");
            return;
        }
        catch (TypesenseApiNotFoundException e)
        {
            Console.WriteLine($"Collection {schemaName} has not been created yet. {e.Message}");
        }

        Schema schema = new(schemaName, new List<Field>
        {
            new("id", FieldType.String),
            new("title", FieldType.String),
            new("content", FieldType.String),
            new("tags", FieldType.StringArray),
            new("createdAt", FieldType.Int64),
            new("hasAcceptedAnswer", FieldType.Bool),
            new("answerCount", FieldType.Int32)
        })
        {
            DefaultSortingField = "createdAt"
        };

        await client.CreateCollection(schema);
        Console.WriteLine($"Collection {schemaName} has been created.");
    }
}