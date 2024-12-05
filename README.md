[![NuGet Package](https://img.shields.io/nuget/v/SeedEasy)](https://www.nuget.org/packages/SeedEasy/)

### Table of content

- [Installing](#Installation)
- [Usage](#Usage)

### Installation

  ```bash
  dotnet add package SeedEasy
  ```

### Usage

1. Create you seeder by implementing 'ISeeder'

``` csharp
public class UserSeeder : Seeder<User>
{
    public override async Task SeedAsync(CancellationToken ct)
    {
        if (DbSet.Any()) return;

        DbSet.Add(new User
        {
            FirstName = "Sara",
            LastName = "Sarara",
        });

        await Context.SaveChangesAsync(ct);
    }
}
```

2. Configure your DbContext

``` csharp
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.AddSeedData(Assembly.GetExecutingAssembly());

        base.OnConfiguring(optionsBuilder);
    }
```
