namespace SeedEasy;

public interface ISeeder<TEntity>
{
    public IEnumerable<TEntity> Generate();
}