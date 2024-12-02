namespace SeedEasy;

public interface ISeeder<out TEntity>
{
    public IEnumerable<TEntity> Generate();
}