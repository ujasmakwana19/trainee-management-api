
using TraineeManagement.Api.UserModel;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
public static class SeederService
{
    public static async Task CreateAdminUser(IServiceProvider serviceProvider)
    {
        PasswordHasher<User> ph = new PasswordHasher<User>();
        User u = new User{

            Username = "admin",
            Email = "admin@mail.com",
            PasswordHash = "Ram",
            Role = UserRole.Admin

        };

        String hashPass = ph.HashPassword(u, "Ram");

        u.PasswordHash = hashPass;

        using IServiceScope scope = serviceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
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