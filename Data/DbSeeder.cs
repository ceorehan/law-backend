using Microsoft.EntityFrameworkCore;
using ZALaw.Api.Models;

namespace ZALaw.Api.Data;

/// <summary>Seeds a demo consultant account so the Consultant Portal can be tested end-to-end.</summary>
public static class DbSeeder
{
    public static async Task SeedAsync(AppDbContext db)
    {
        if (await db.Users.AnyAsync(u => u.Role == UserRole.Consultant)) return;

        db.Users.Add(new User
        {
            FullName = "Sara Ahmed",
            Email = "sara.ahmed@zalawassociates.pk",
            PhoneNumber = "0300-0000000",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("ChangeMe123!"),
            Role = UserRole.Consultant,
        });

        await db.SaveChangesAsync();
    }
}
