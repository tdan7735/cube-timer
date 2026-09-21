using backend.Models;
using Microsoft.EntityFrameworkCore;

namespace backend.Data;

public static class UserSeeder {
    public const string DefaultUsername = "me";

    public static async Task SeedAsync(AppDbContext context) {
        if (await context.Users.AnyAsync(user => user.Username == DefaultUsername)) {
            return;
        }

        context.Users.Add(new User { Username = DefaultUsername });
        await context.SaveChangesAsync();
    }
}
