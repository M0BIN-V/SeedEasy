using Microsoft.EntityFrameworkCore;

namespace SeedEasy;

public abstract class Seeder<TEntity> where TEntity : class
{
    public required  DbContext Context{ get; init; } 
    public DbSet<TEntity> DbSet => Context.Set<TEntity>();
    
    public abstract Task SeedAsync(CancellationToken ct);
}