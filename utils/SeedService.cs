
using TraineeManagement.Api.UserModel;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
public static class SeederService
{
    public static async Task CreateAdminUser(IServiceProvider serviceProvider)
    {
        PasswordHasher<User> ph = new PasswordHasher<User>();
        User u = new User{

            Username = "admin",
            Email = "admin@mail.com",
            Role = UserRole.Admin,
            CreatedDate = DateTime.UtcNow,
            UpdatedDate = DateTime.UtcNow

        };

        String hashPass = ph.HashPassword(u, "Ram");

        u.PasswordHash = hashPass;

        var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                context.Users.Add(u);
                await context.SaveChangesAsync();
                System.Console.WriteLine("Admin Created Successfully");
            }
            else
            {
                System.Console.WriteLine("Admin Already Exists ");
            }
        }
        catch (System.Exception)
        {
            System.Console.WriteLine("Failed to create admin");
            throw;
        }
    }
}