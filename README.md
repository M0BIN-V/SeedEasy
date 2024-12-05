[![NuGet Package](https://img.shields.io/nuget/v/SeedEasy)](https://www.nuget.org/packages/SeedEasy/)

### Table of content

- [Installing](#Installation)
- [Usage](#Usage)
- [More Complex Examples](#More-Complex-Examples)

### Installation

  ```bash
  dotnet add package SeedEasy
  ```

### Usage

Got it! Here's a revised version of your documentation with added details and explanations:

---

### 1. Create Your Seeder by Implementing `Seeder<>`

To automate the process of seeding initial data into the database, you can implement the `Seeder<>` class for your
entities.

```csharp
public class UserSeeder : Seeder<User>
{
    public override async Task SeedAsync(CancellationToken ct)
    {
        if (DbSet.Any()) return;

        // Add initial data
        DbSet.Add(new User
        {
            FirstName = "Sara",
            LastName = "Sarara",
        });

        // Save changes to the database
        await Context.SaveChangesAsync(ct);
    }
}
```

### 2. Configure Your `DbContext` to Load Seed Data

Next, configure your `DbContext` to automatically load the seed data from your assembly during the application startup.

```csharp
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    // Load seed data from the current assembly
    optionsBuilder.AddSeedData(Assembly.GetExecutingAssembly());

    base.OnConfiguring(optionsBuilder);
}
```

- `AddSeedData` automatically finds and executes all the seeders in the current assembly, ensuring that seed data is
  populated when the application starts.
- This approach centralizes the seeding logic and allows you to add seed data without manually invoking seed methods.

### 3. More Complex Examples

In some cases, you may need to seed more complex data, such as creating relationships between entities. Here’s an
example:

```csharp
public class UserSeeder : Seeder<User>
{
    public override async Task SeedAsync(CancellationToken ct)
    {
        if (DbSet.Any()) return;

        var user = new User
        {
            FirstName = "Sara",
            LastName = "Sarara",
            // Adding related entities
            Roles = new List<Role>
            {
                new Role { Name = "Admin" },
                new Role { Name = "User" }
            }
        };

        DbSet.Add(user);
        await Context.SaveChangesAsync(ct);
    }
}
```

- In this example, we’re seeding a `User` entity with related `Role` entities. This shows how to seed complex data with
  relationships between entities.
