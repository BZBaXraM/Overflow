using Microsoft.EntityFrameworkCore.Design;

namespace QuestionService.Data;

/// <summary>
/// Design-time factory for Entity Framework tools
/// </summary>
public class QuestionContextFactory : IDesignTimeDbContextFactory<QuestionContext>
{
    public QuestionContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<QuestionContext>();
        
        // Use a design-time connection string for EF tools
        // This won't be used at runtime - only for migrations and scaffolding
        optionsBuilder.UseNpgsql("Host=localhost;Database=questionDb_design;Username=postgres;Password=postgres");
        
        return new QuestionContext(optionsBuilder.Options);
    }
}