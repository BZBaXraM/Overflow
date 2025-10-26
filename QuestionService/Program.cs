using Microsoft.EntityFrameworkCore;
using QuestionService.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.AddServiceDefaults();
builder.Services.AddAuthentication()
    .AddKeycloakJwtBearer("keycloak", "overflow", options =>
    {
        options.RequireHttpsMetadata = false;
        options.Audience = "overflow";
    });

builder.AddNpgsqlDbContext<QuestionContext>("questionDb");

// builder.Services.AddDbContext<QuestionContext>(options =>
// {
//     options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
// });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.MapControllers();

app.MapDefaultEndpoints();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<QuestionContext>();
    // await context.Database.EnsureDeletedAsync();
    await context.Database.MigrateAsync();
}
catch (Exception e)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(e, "An error occurred while migrating or seeding the database.");
}

app.Run();