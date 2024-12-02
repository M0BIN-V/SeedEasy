using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace SeedEasy;

public static class OptionBuilderExtensions
{
    public static DbContextOptionsBuilder AddSeedData(this DbContextOptionsBuilder optionsBuilder , Assembly seedersAssembly)
    {
        var seederTypes = seedersAssembly
            .GetTypes()
            .Where(t =>
                t is { IsClass: true, IsAbstract: false, IsInterface: false } &&
                t.GetInterfaces().Any(i =>
                    i.IsGenericType &&
                    i.GetGenericTypeDefinition() == typeof(ISeeder<>)))
            .ToList();

        optionsBuilder.UseAsyncSeeding(async (context, _, ct) =>
        {
            foreach (var seederType in seederTypes)
            {
                var seederInterface = seederType
                    .GetInterfaces()
                    .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ISeeder<>));

                if (seederInterface is null) continue;

                var entityType = seederInterface.GetGenericArguments()[0];

                var seederInstance = Activator.CreateInstance(seederType);
                if (seederInstance is null) continue;

                var generateMethod = seederInterface.GetMethod(nameof(ISeeder<object>.Generate));

                var seedData = generateMethod?.Invoke(seederInstance, null);
                if (seedData is null) continue;

                var setMethod = context.GetType()
                    .GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .FirstOrDefault(m =>
                        m is { Name: nameof(context.Set), IsGenericMethod: true } &&
                        m.GetGenericArguments().Length is 1);

                var set = setMethod?.MakeGenericMethod(entityType).Invoke(context, null);
                if (set == null) continue;

                var addRangeAsyncMethod = set.GetType().GetMethods()
                    .FirstOrDefault(m =>
                        m.Name == nameof(DbSet<object>.AddRangeAsync) &&
                        m.GetParameters().Length == 2 &&
                        m.GetParameters()[1].ParameterType == typeof(CancellationToken));

                if (addRangeAsyncMethod == null) continue;

                await (Task)addRangeAsyncMethod.Invoke(set, [seedData, ct])!;
            }

            await context.SaveChangesAsync(ct);
        });

        return optionsBuilder;
    }
}