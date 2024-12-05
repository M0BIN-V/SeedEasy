using System.Reflection;
using Microsoft.EntityFrameworkCore;

namespace SeedEasy;

public static class OptionBuilderExtensions
{
    public static DbContextOptionsBuilder AddSeedData(this DbContextOptionsBuilder optionsBuilder,
        Assembly seedersAssembly)
    {
        seedersAssembly
            .GetTypes()
            .Where(t => t is
                        {
                            IsClass: true,
                            IsAbstract: false,
                            IsInterface: false,
                            BaseType.IsGenericType: true
                        } &&
                        t.BaseType.GetGenericTypeDefinition() == typeof(Seeder<>))
            .ToList()
            .ForEach(seederType => optionsBuilder.UseAsyncSeeding((context, _, ct) =>
            {
                var seederInstance = Activator.CreateInstance(seederType);
                var contextProperty = seederType.GetProperty(nameof(Seeder<object>.Context));
                contextProperty!.SetValue(seederInstance, context);

                var seederMethod = seederType.GetMethod(nameof(Seeder<string>.SeedAsync));

                return (Task)seederMethod!.Invoke(seederInstance, [ct])!;
            }));

        return optionsBuilder;
    }
}