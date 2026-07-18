
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
        User[] users = [
            new User{
                Username = "admin",
                Email = "admin@mail.com",
                Role = UserRole.Admin
            },
            new User{
                Username = "mentor",
                Email = "mentor@mail.com",
                Role = UserRole.Mentor
            },
            new User{
                Username = "trainee",
                Email = "trainee@mail.com",
                Role = UserRole.Trainee
            }
        ];

        foreach (User u in users)
        {
            u.PasswordHash = ph.HashPassword(u, "Ram");
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        try
        {
            if (!await context.Users.AnyAsync(u => u.Username == "admin"))
            {
                context.Users.AddRange(users);
                await context.SaveChangesAsync();
                Console.WriteLine("[Seeder-Service]::::Admin Created Successfully");
            }
            else
            {
                Console.WriteLine("[Seeder-Service]::::Admin Already Exists ");
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
                Console.WriteLine("[Seeder-Service]::::Trainee Already Exists ");
            }

            if (!await context.Mentors.AnyAsync())
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
                Console.WriteLine("[Seeder-Service]::::Mentor Already Exists ");
            }

            if (!await context.LearningTasks.AnyAsync())
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
                Console.WriteLine("[Seeder-Service]::::Learning Tasks Already Exists ");
            }

        }
        catch (Exception)
        {
            Console.WriteLine("[Seeder-Service]:::: Failed to seed data..");
            throw;
        }
    }
}