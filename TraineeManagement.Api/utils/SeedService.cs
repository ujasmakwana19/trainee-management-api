using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using TraineeManagement.Data.DataBaseContext;
using TraineeManagement.Data.MentorModel;
using TraineeManagement.Data.TaskModel;
using TraineeManagement.Data.TraineeModel;
using TraineeManagement.Data.UserModel;

public static class SeederService
{
    public static async Task SeedData(IServiceProvider serviceProvider)
    {
        PasswordHasher<User> ph = new PasswordHasher<User>();

        User adminUser = new User
        {
            Username = "admin",
            Email = "admin@mail.com",
            Role = UserRole.Admin
        };

        User[] mentorUsers = [
            new User { Username = "mentor_lokesh", Email = "lg@gmail.com", Role = UserRole.Mentor },
            new User { Username = "mentor_kaushal", Email = "khb@gmail.com", Role = UserRole.Mentor },
            new User { Username = "mentor_raj", Email = "striver@mail.com", Role = UserRole.Mentor }
        ];

        User[] traineeUsers = [
            new User { Username = "trainee_madhav", Email = "mv@mail.com", Role = UserRole.Trainee },
            new User { Username = "trainee_priyanshu", Email = "pb@mail.com", Role = UserRole.Trainee },
            new User { Username = "trainee_sharad", Email = "shp@mail.com", Role = UserRole.Trainee }
        ];

        // Hash passwords for all users
        List<User> allUsers = new List<User> { adminUser };
        allUsers.AddRange(mentorUsers);
        allUsers.AddRange(traineeUsers);

        foreach (User u in allUsers)
        {
            u.PasswordHash = ph.HashPassword(u, "Ram");
        }

        using IServiceScope scope = serviceProvider.CreateScope();
        AppDbContext context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        try
        {
            if (!await context.Users.AnyAsync(u => u.Role == UserRole.Admin))
            {
                context.Users.AddRange(allUsers);
                await context.SaveChangesAsync();
                Console.WriteLine("[Seeder-Service]:::: Users Created Successfully");
            }
            else
            {
                Console.WriteLine("[Seeder-Service]:::: Admin / Users Already Exist");
            }

            if (!await context.Mentors.AnyAsync())
            {
                context.Mentors.AddRange(
                    new Mentor 
                    { 
                        FirstName = "Lokesh", 
                        LastName = "Gangani", 
                        Email = "lg@gmail.com", 
                        Expertise = "OS, DBMS, CN", 
                        Status = MentorStatus.Active, 
                        User = mentorUsers[0] 
                    },
                    new Mentor 
                    { 
                        FirstName = "Kaushal", 
                        LastName = "Bhavsar", 
                        Email = "khb@gmail.com", 
                        Expertise = "Problem solving", 
                        Status = MentorStatus.Inactive, 
                        User = mentorUsers[1] 
                    },
                    new Mentor 
                    { 
                        FirstName = "Raj", 
                        LastName = "Vikramaditya", 
                        Email = "striver@mail.com", 
                        Expertise = "DSA", 
                        Status = MentorStatus.Active, 
                        User = mentorUsers[2] 
                    }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("[Seeder-Service]:::: Mentors Created Successfully");
            }
            else
            {
                Console.WriteLine("[Seeder-Service]:::: Mentors Already Exist");
            }

            if (!await context.Trainees.AnyAsync())
            {
                context.Trainees.AddRange(
                    new Trainee 
                    { 
                        FirstName = "Madhav", 
                        LastName = "Visani", 
                        Email = "mv@mail.com", 
                        TechStack = "Python", 
                        Status = StatusValue.Inactive, 
                        User = traineeUsers[0] 
                    },
                    new Trainee 
                    { 
                        FirstName = "Priyanshu", 
                        LastName = "Baraiya", 
                        Email = "pb@mail.com", 
                        TechStack = "React js", 
                        Status = StatusValue.Inactive, 
                        User = traineeUsers[1] 
                    },
                    new Trainee 
                    { 
                        FirstName = "Sharad", 
                        LastName = "Barad", 
                        Email = "shp@mail.com", 
                        TechStack = "Nodejs", 
                        Status = StatusValue.Inactive, 
                        User = traineeUsers[2] 
                    }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("[Seeder-Service]:::: Trainees Created Successfully");
            }
            else
            {
                Console.WriteLine("[Seeder-Service]:::: Trainees Already Exist");
            }

            // Seed Learning Tasks
            if (!await context.LearningTasks.AnyAsync())
            {
                context.LearningTasks.AddRange(
                    new LearningTask 
                    { 
                        Title = "Backend API Development", 
                        Description = "APIs, Controller, Service, DTO, Middleware implementation", 
                        ExpectedTechStack = "ASP .NET Web API", 
                        DueDate = DateTime.UtcNow.AddDays(7), 
                        Status = TaskStatusValue.Draft 
                    },
                    new LearningTask 
                    { 
                        Title = "Frontend UI Components", 
                        Description = "Reusable Components, Hooks, State management", 
                        ExpectedTechStack = "React", 
                        DueDate = DateTime.UtcNow.AddDays(10), 
                        Status = TaskStatusValue.Published 
                    },
                    new LearningTask 
                    { 
                        Title = "Database Schemas & Indexing", 
                        Description = "CRUD, Primary key, Foreign key, Constraints, Performance Optimization", 
                        ExpectedTechStack = "MySQL Server", 
                        DueDate = DateTime.UtcNow.AddDays(3), 
                        Status = TaskStatusValue.Closed 
                    }
                );
                await context.SaveChangesAsync();
                Console.WriteLine("[Seeder-Service]:::: Learning Tasks Created Successfully");
            }
            else
            {
                Console.WriteLine("[Seeder-Service]:::: Learning Tasks Already Exist");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Seeder-Service]:::: Failed to seed data. Error: {ex.Message}");
            throw;
        }
    }
}