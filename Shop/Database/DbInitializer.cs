using Exam2.Backend.Entities;
using Exam2.Backend.Data;

namespace Exam2.Backend;

public static class DbInitializer
{
    public static void Initialize(ApplicationDbContext context)
    {
        if (context.Categories.Any()) return;

        var categories = new Category[]
        {
            new Category { Name = "Laptops", ImageUrl = "laptops.png" },
            new Category { Name = "Smartphones", ImageUrl = "phones.png" },
            new Category { Name = "Accessories", ImageUrl = "accessories.png" }
        };

        context.Categories.AddRange(categories);
        context.SaveChanges();
    }
}
