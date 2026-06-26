
using TraineeManagement.Data.UserModel;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Data.DataBaseContext;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.TraineeModel;
using TraineeManagement.Data.MentorModel;
using TraineeManagement.Data.TaskModel;
public static class SeederService
{
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        PasswordHasher<User> ph = new PasswordHasher<User>();
        User u = new User
        {

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

            if (!await context.Trainees.AnyAsync())
            {
                context.Trainees.AddRange(
                    new Trainee { FirstName = "Madhav", LastName = "Visani", Email = "mv@mail.com", TechStack = "Python", Status = StatusValue.Inactive },
                    new Trainee { FirstName = "Priyanshu", LastName = "Baraiya", Email = "pb@mail.com", TechStack = "React js", Status = StatusValue.Inactive },
                    new Trainee { FirstName = "Sharad", LastName = "Barad", Email = "shp@mail.com", TechStack = "NOdejs", Status = StatusValue.Inactive }
                );
                await context.SaveChangesAsync();
            }
            else
            {
                System.Console.WriteLine("Trainee Already Exists ");
            }

            if (!context.Mentors.Any())
            {
                context.Mentors.AddRange(
                    new Mentor { FirstName = "Lokesh", LastName = "Gangani", Email = "lg@gmail.com", Expertise = "OS, DBMS, CN", Status = MentorStatus.Active },
                    new Mentor { FirstName = "Kaushal", LastName = "Bhavsar", Email = "khb@gmail.com", Expertise = "Problem solving", Status = MentorStatus.Inactive },
                    new Mentor { FirstName = "Raj", LastName = "Vikramaditya", Email = "striver@mail.com", Expertise = "DSA", Status = MentorStatus.Active }
                );
                await context.SaveChangesAsync();
            }
            else
            {
                System.Console.WriteLine("Mentor Already Exists ");
            }

            if (!context.LearningTasks.Any())
            {
                context.LearningTasks.AddRange(
                    new LearningTask { Title = "Backend", Description = "APIs, Controller, Sevice, Dto, Middleware", ExpectedTechStack = "ASP .NET Web API", DueDate = DateTime.UtcNow, Status = TaskStatusValue.Draft },
                    new LearningTask { Title = "Fronend", Description = "Components, Hooks, Methods", ExpectedTechStack = "React", DueDate = DateTime.UtcNow, Status = TaskStatusValue.Published },
                    new LearningTask { Title = "Database", Description = "CRUD, primary key, foreign key, Unique, Auto generated", ExpectedTechStack = "MySQL Server", DueDate = DateTime.UtcNow, Status = TaskStatusValue.Closed }
                );
                await context.SaveChangesAsync();
            }
            else
            {
                System.Console.WriteLine("Learning Tasks Already Exists ");
            }

        }
        catch (System.Exception)
        {
            System.Console.WriteLine("Failed to seed data..");
            throw;
        }
    }
}