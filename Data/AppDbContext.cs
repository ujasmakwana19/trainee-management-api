using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using TraineeManagement.Api.TraineeModel;
using TraineeManagement.Api.UserModel;
namespace TraineeManagement.Api.Data;
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }
    protected override void OnModelCreating(ModelBuilder builder)
        {
            
            base.OnModelCreating(builder);

            // Store the Enum as a String (e.g., "Admin") instead of an Integer (0)
            builder.Entity<User>()
                .Property(u => u.Role)
                .HasConversion<string>();
        }
    public DbSet<Trainee> Trainees {get; set;}
    public DbSet<User> Users {get; set;}
}