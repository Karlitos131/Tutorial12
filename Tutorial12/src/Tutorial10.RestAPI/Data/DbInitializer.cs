using Tutorial10.Data.Models;

namespace Tutorial10.Data;

public class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        if (!context.Roles.Any())
        {
            context.Roles.AddRange(
                new Role { Name = "Admin" },
                new Role { Name = "User" }
            );
            context.SaveChanges();
        }
    }
}