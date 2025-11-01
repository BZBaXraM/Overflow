using Common;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Wolverine.RabbitMQ;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();

await builder.UseWolverineWithRabbitMqAsync(opts =>
{
    opts.ListenToRabbitQueue("questions.search", cfg =>
    {
        cfg.BindExchange("questions");
    });
    opts.ApplicationAssembly = typeof(Program).Assembly;
});

var typeSenseUri = builder.Configuration["services:typesense:typesense:0"];

if (string.IsNullOrEmpty(typeSenseUri))
{
    throw new InvalidOperationException("Typesense URI not found");
}

var typeSenseApiKey = builder.Configuration["typesense-api-key"];
if (string.IsNullOrEmpty(typeSenseApiKey))
{
    throw new InvalidOperationException("Typesense API key not found");
}

var uri = new Uri(typeSenseUri);
builder.Services.AddTypesenseClient(config =>
{
    config.ApiKey = typeSenseApiKey;
    config.Nodes = new List<Node>
    {
        new(uri.Host, uri.Port.ToString(), uri.Scheme)
    };
});


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapDefaultEndpoints();

app.MapGet("/api/search", async (string query, ITypesenseClient client) =>
{
    string? tag = null;
    var tagMatch = Regex.Match(query, @"\[(.*?)\]");
    if (tagMatch.Success)
    {
        tag = tagMatch.Groups[1].Value;
        query = query.Replace(tagMatch.Value, "").Trim();
    }

    SearchParameters searchParams = new(query, "title,content");

    if (!string.IsNullOrWhiteSpace(tag))
    {
        searchParams.FilterBy = $"tags:=[{tag}]";
    }

    try
    {
        var result = await client.Search<SearchQuestion>("questions", searchParams);
        return Results.Ok(result.Hits.Select(x => x.Document));
    }
    catch (Exception e)
    {
        return Results.Problem("Typesense search failed", e.Message);
    }
});

app.MapGet("/api/search/similar-titles", async (string query, ITypesenseClient client) =>
{
    SearchParameters searchParameters = new(query, "title");

    try
    {
        var result = await client.Search<SearchQuestion>("questions", searchParameters);
        return Results.Ok(result.Hits.Select(x => x.Document));
    }
    catch (Exception e)
    {
        return Results.Problem("Typesense search failed", e.Message);
    }
});

using var scope = app.Services.CreateScope();
var client = scope.ServiceProvider.GetRequiredService<ITypesenseClient>();
await SearchInitializer.EnsureIndexExistsAsync(client);

app.Run();